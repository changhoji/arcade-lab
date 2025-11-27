import { Server } from "socket.io";
import { AuthService } from '../services/authService';
import { ServerService } from "../services/serverService";
import { setupAuthNamespace } from "./auth";
import { setupLobbyNamespace } from "./lobby";


export function setupNameSpaces(io: Server) {
    const serverService = new ServerService();
    const authService = new AuthService();

    const auth = io.of(`/auth`);
    setupAuthNamespace(auth, serverService, authService);

    serverService.createLobby("1");
    serverService.createLobby("2");

    const lobby = io.of(`/lobby`);
    setupLobbyNamespace(lobby, serverService);
}