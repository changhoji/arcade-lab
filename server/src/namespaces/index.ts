import { Server } from "socket.io";
import { LobbyManager } from "../managers/lobbyManager";
import { PlayerManager } from "../managers/playerManager";
import { SetupAuthNamespace } from "./auth";
import { setupLobbyNamespace } from "./lobby";

export function setupNameSpaces(io: Server) {
    const playerManager = new PlayerManager();
    const lobbyManager = new LobbyManager();

    SetupAuthNamespace(io, playerManager);

    const mainLobby = io.of("/lobby");
    setupLobbyNamespace(mainLobby, playerManager, lobbyManager);
}