import { Server } from "socket.io";
import { LobbyManager } from "../managers/lobbyManager";
import { PlayerManager } from "../managers/playerManager";
import { RoomManager } from "../managers/roomManager";
import { SetupAuthNamespace } from "./auth";
import { setupLobbyNamespace } from "./lobby";

export function setupNameSpaces(io: Server) {
    const playerManager = new PlayerManager();
    const roomManager = new RoomManager();
    const lobbyManager = new LobbyManager();

    SetupAuthNamespace(io, playerManager);

    const lobby = io.of("/lobby");
    setupLobbyNamespace(lobby, playerManager, lobbyManager, roomManager);
}