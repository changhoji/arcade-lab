import { Namespace } from "socket.io";
import { AuthService } from "../services/authService";
import { ServerService } from '../services/serverService';
import { PlayerState } from "../types/common";
import { generateId } from "../utils/idGenerator";

export function setupAuthNamespace(
    io: Namespace,
    serverService: ServerService,
    authService: AuthService
) {
    io.on("connection", (socket) => {
        // guest signin request
        socket.on("auth:guest", (callback: (player: PlayerState) => void) => {
            const userId = generateId();
            const player = authService.addPlayer(socket.id, userId);
            callback(player);
            console.log(`Guest signin: ${userId}`);
        })

        // client disconnected
        socket.on("disconnect", () => {
            const userId = authService.getUserIdBySocket(socket.id);
            if (userId) {
                authService.removePlayer(userId);
            }
            console.log(`player ${userId} disconnected from auth`);
        })
    })
}
