import { Namespace } from "socket.io";
import { ServerService } from '../services/serverService';
import { Lobby } from "../types/lobby";
import { generateId } from "../utils/idGenerator";

export function setupLobbyNamespace(namespace: Namespace, serverService: ServerService) {
    namespace.on("connection", (socket) => {
        // get userId
        const userId = socket.handshake.auth.userId;
        if (!userId) {
            socket.disconnect();
            return;
        }

        socket.on("lobby:list", (callback: (lobbies: Lobby[]) => void) => {
            const lobbies = serverService.getLobbyDatas();
            callback(lobbies);
        });

        socket.on("lobby:create", (name: string, callback: (lobbyId: string | null) => void) => {
            const lobbyId = generateId();
            const lobby = serverService.createLobby(lobbyId, name);
            if (lobby) {
                lobby.joinLobby(userId);
                socket.join(lobbyId)
                callback(lobbyId);
            } else {
                callback(null);
            }
        });

        socket.on("lobby:join", (lobbyId: string, callback: (lobbyId: string | null) => void) => {
            const lobby = serverService.getLobby(lobbyId);
            if (lobby) {
                lobby.joinLobby(userId);
                socket.join(lobbyId)
                console.log(`join to ${lobbyId}`);
                console.log(`join lobby, room length: ${socket.rooms.size}`);
                
                callback(lobbyId);
            } else {
                callback(null);
            }
        })

        socket.on("disconnecting", () => {
            socket.rooms.forEach((roomId) => {
                if (roomId !== socket.id) {
                    const lobby = serverService.getLobby(roomId);
                    if (lobby) {
                        lobby.leaveLobby(userId);
                        if (lobby.currentPlayers === 0) {
                            console.log(`lobby ${lobby} removed`);
                            serverService.removeLobby(roomId);
                        }
                    }
                }
            })
        })

        // // find lobby service
        // const lobbyId = socket.nsp.name.split('/')[-1];
        // const lobbyService = serverService.getLobby(lobbyId);
        // if (!lobbyService) {
        //     console.error("LobbyService not exist");
        //     return;
        // }

        // const player = lobbyService.addLobbyPlayer(userId);
        // const otherPlayers = lobbyService.getOtherPlayers(userId);

        // socket.emit("player:connect", player);
        // socket.emit("player:others", otherPlayers);
        // socket.broadcast.emit("player:join", player);

        // socket.on("player:move", (position: Position) => {
        //     if (lobbyService.updatePosition(userId, position)) {
        //         const moveData: PlayerMoveData = {
        //             userId,
        //             position
        //         }
        //         socket.broadcast.emit("player:move", moveData);
        //     }
        // });

        // lobby create request
        // socket.on("lobby:create", () => {
        //     const lobbyId = generateId();
        //     const lobby = serverService.createLobby(lobbyId);
        //     if (lobby) {
        //         socket.join(`/lobby/${lobbyId}`)

        //         // callback with success
                
        //         // send lobby to others
        //     } else {
        //         // callback with fail
        //     }
        // })

        // // lobby join request
        // socket.on("lobby:join", (lobbyId: string) => {
        //     const lobby = serverService.getLobby(lobbyId);
        //     if (lobby) {
        //         socket.join(`/lobby/${lobbyId}`)

        //         // callback with success

        //         // send joined to others
        //     } else {
        //         // callback with fail
        //     }
        // })
    })
}