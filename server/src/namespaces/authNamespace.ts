import { Namespace, Server } from "socket.io";
import { AuthService } from "../services/authService";
import { ServerService } from '../services/serverService';
import { PlayerState } from "../types/common";
import { generateId } from "../utils/idGenerator";

export class AuthNamespace {
    private namespace: Namespace
    constructor(
        private io: Server,
        private serverService: ServerService,
        private authService: AuthService
    ) {
        this.namespace = io.of("/auth");
        this.registerHandlers();
    }

    private registerHandlers() {
        this.namespace.on("connection", (socket) => {
            // guest signin request
            socket.on("auth:guest", (callback: (player: PlayerState) => void) => {
                const userId = generateId();
                const player = this.authService.addPlayer(socket.id, userId);
                callback(player);
                console.log(`Guest signin: ${userId}`);
            });

            // client disconnected
            socket.on("disconnect", () => {
                const userId = this.authService.getUserIdBySocket(socket.id);
                if (userId) {
                    this.authService.removePlayer(userId);
                }
                console.log(`player ${userId} disconnected from auth`);
            });
        });
    }
    
    
}
