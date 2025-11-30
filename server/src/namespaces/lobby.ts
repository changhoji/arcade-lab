import { Namespace, Server } from "socket.io";
import { AuthService } from "../services/authService";
import { LobbyService } from "../services/lobbyService";
import { ServerService } from '../services/serverService';
import { Position } from "../types/common";
import { Lobby, LobbyPlayerSnapshot } from "../types/lobby";
import { generateId } from "../utils/idGenerator";

export class LobbyNamespace {
    private namespace: Namespace;

    constructor(
        private io: Server,
        private serverService: ServerService,
        private authService: AuthService,
    ) {
        this.namespace = io.of("/lobby");
        this.temp();
    }

    temp() {
        this.namespace.on("connection", (socket) => {
            // get userId
            var userId: string = "";
            userId = socket.handshake.auth.userId;

            var lobbyService: LobbyService | null = null;

            socket.on("lobby:list", (callback: (lobbies: Lobby[]) => void) => {
                const lobbies = this.serverService.getLobbyDatas();
                callback(lobbies);
            });

            socket.on("lobby:create", (name: string, callback: (lobbyId: string | null) => void) => {
                const lobbyId = generateId();
                const lobby = this.serverService.createLobby(lobbyId, name);
                if (lobby) {
                    lobby.joinLobby(userId);
                    socket.join(lobbyId);
                    lobbyService = lobby;
                    callback(lobbyId);
                } else {
                    callback(null);
                }
            });

            socket.on("lobby:join", (lobbyId: string, callback: (lobbyId: string | null) => void) => {
                const lobby = this.serverService.getLobby(lobbyId);
                if (lobby) {
                    lobby.joinLobby(userId);
                    socket.join(lobbyId)
                    lobbyService = lobby;
                    callback(lobbyId);
                } else {
                    callback(null);
                }
            });

            socket.on("lobby:init", (callback: (players: LobbyPlayerSnapshot[]) => void) => {
                if (lobbyService) {
                    const players = lobbyService.getPlayerSnapshots();
                    callback(players);

                    socket.to(lobbyService.lobbyId).emit("player:joined", lobbyService.getPlayerSnapshot(userId));
                }
            });

            socket.on("player:moved", (position: Position) => {
                if (lobbyService) {
                    if (lobbyService.updatePosition(userId, position)) {
                        console.log(`emit ${userId}`);
                        socket.to(lobbyService.lobbyId).emit("player:moved",
                            userId,
                            position
                        );
                    }
                }
            });

            socket.on("player:moving", (isMoving: boolean) => {
                if (lobbyService) {
                    if (lobbyService.updateIsMoving(userId, isMoving)) {
                        socket.to(lobbyService.lobbyId).emit("player:moving",
                            userId,
                            isMoving
                        );
                    }
                }
            });

            socket.on("player:changeSkin", (skinIndex: number) => {
                if (this.authService && lobbyService) {
                    if (this.authService.updateSkinIndex(userId, skinIndex)) {
                        socket.to(lobbyService.lobbyId).emit("player:skinChanged", userId, skinIndex);
                    }
                }
            });

            socket.on("disconnecting", () => {
                if (lobbyService) {
                    lobbyService.leaveLobby(userId);
                    socket.to(lobbyService.lobbyId).emit("player:left", userId);
                    if (lobbyService.currentPlayers === 0) {
                        console.log(`lobby ${lobbyService} removed`);
                        this.serverService.removeLobby(lobbyService.lobbyId);
                    }
                }
            })
        });
    }
    

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
}