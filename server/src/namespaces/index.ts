import { Server } from "socket.io";
import { AuthService } from '../services/authService';
import { ServerService } from "../services/serverService";
import { setupAuthNamespace } from "./auth";
import { LobbyNamespace } from "./lobby";


export function setupNameSpaces(io: Server) {
    const authService = new AuthService();
    const serverService = new ServerService(authService);

    const auth = io.of(`/auth`);
    setupAuthNamespace(auth, serverService, authService);

    serverService.createLobby("1", "test1");
    serverService.createLobby("2", "test2");

    const lobby = io.of(`/lobby`);
    new LobbyNamespace(io, serverService, authService);
}