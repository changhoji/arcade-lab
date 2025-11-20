import { Namespace } from "socket.io";
import { LobbyManager } from "../managers/lobbyManager";
import { PlayerManager } from "../managers/playerManager";
import { RoomManager } from "../managers/roomManager";
import { Position } from "../types/common";
import { CreateRoomRequest, PlayerMoveData, PlayerNicknameData, PlayerSkinData } from "../types/lobby";

export function setupLobbyNamespace(namespace: Namespace, playerManager: PlayerManager, lobbyManager: LobbyManager, roomManager: RoomManager) {
    namespace.on("connection", (socket) => {
        const userId = socket.handshake.auth.userId;
        if (!userId) {
            socket.disconnect();
            return;
        }

        playerManager.updateNamespace(userId, "/lobby");
        const player = lobbyManager.addPlayer(userId);
        const otherPlayers = lobbyManager.getOtherPlayers(userId);

        socket.emit("player:connect", player);
        socket.emit("player:others", otherPlayers);
        socket.broadcast.emit("player:join", player);

        socket.on("player:move", (position: Position) => {
            if (lobbyManager.updatePosition(userId, position)) {
                const moveData: PlayerMoveData = {
                    userId,
                    position
                }
                socket.broadcast.emit("player:move", moveData);
            }
        });

        socket.on("player:skin", (skinIndex: number) => {
            if (lobbyManager.updateSkin(userId, skinIndex)) {
                const skinData: PlayerSkinData = {
                    userId,
                    skinIndex
                };
                socket.broadcast.emit("player:skin", skinData)
            }
        });

        socket.on("player:nickname", (nickname: string) => {
            if (lobbyManager.updateNickname(userId, nickname)) {
                const nicknameData: PlayerNicknameData = {
                    userId,
                    nickname
                };
                socket.broadcast.emit("player:nickname", nicknameData);
            }
        })

        socket.on("room:create", (request: CreateRoomRequest) => {
            const room = roomManager.createRoom(userId, request);
            namespace.emit("room:create", room);
        });

        socket.on("room:remove", (roomId: string) => {
            roomManager.removeRoom(roomId);
            namespace.emit("room:remove", roomId);
        })

        socket.on("disconnect", () => {
            console.log("player disconnected from lobby");
            lobbyManager.removePlayer(userId);
            socket.broadcast.emit("player:leave", userId);
        })
    })
}