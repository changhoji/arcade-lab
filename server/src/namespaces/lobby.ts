import { Namespace } from "socket.io";
import { LobbyManager } from "../managers/lobbyManager";
import { PlayerManager } from "../managers/playerManager";
import { Position } from "../types/common";

export function setupLobbyNamespace(namespace: Namespace, playerManager: PlayerManager, lobbyManager: LobbyManager) {
    namespace.on("connection", (socket) => {
        const userId = socket.handshake.auth.userId; 
        if (!userId) {
            socket.disconnect();
            return;
        }

        console.log(`Player ${userId} connected to main lobby`);

        playerManager.updateNamespace(userId, "/lobby");
        const player = lobbyManager.addPlayer(userId, socket.id);

        const otherPlayers = lobbyManager.getOtherPlayers(userId);
        socket.emit("player:others", otherPlayers);
        socket.broadcast.emit("player:joined", player);    

        socket.on("player:move", (position: Position) => {
            console.log(`player moved to (${position.x}, ${position.y})`)
            if (lobbyManager.updatePosition(userId, position)) {
                socket.broadcast.emit("player:moved", {
                    userId,
                    x: position.x,
                    y: position.y
                });
            }
        });

        socket.on("disconnect", () => {
            console.log("player disconnected from lobby");
            lobbyManager.removePlayer(userId);
            socket.broadcast.emit("player:left", userId);
        })
    })
}