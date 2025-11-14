import { Server } from "socket.io";
import { v4 as uuidv4 } from 'uuid';
import { PlayerManager } from "../managers/playerManager";

export function SetupAuthNamespace(io: Server, playerManager: PlayerManager) {
    io.on("connection", (socket) => {
        socket.on("signin:guest", () => {
            const userId = uuidv4();
            socket.emit("signin:success", userId);
            console.log(`Guest signin: ${userId}`);
        })
    })
}