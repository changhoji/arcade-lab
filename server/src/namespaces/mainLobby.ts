import { Namespace } from "socket.io";
import { MainLobbyManager } from "../managers/MainLobbyManager";
import { PlayerManager } from "../managers/PlayerManager";
import { Position } from "../types/common";

export function setupMainLobbyNamespace(namespace: Namespace, playerManager: PlayerManager, lobbyManager: MainLobbyManager) {
    namespace.on("connection", (socket) => {
        const userId = socket.handshake.auth.userId; 
        if (!userId) {
            socket.disconnect();
            return;
        }

        console.log(`Player ${userId} connected to main lobby`);

        playerManager.updateNamespace(userId, "/mainlobby");
        const player = lobbyManager.addPlayer(userId, socket.id);

        const otherPlayers = lobbyManager.getOtherPlayers(userId);
        socket.emit("player:others", otherPlayers);
        console.log("emitted");

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
    })
}