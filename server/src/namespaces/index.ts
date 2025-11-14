import { Server } from "socket.io";
import { MainLobbyManager } from "../managers/MainLobbyManager";
import { PlayerManager } from "../managers/PlayerManager";
import { SetupAuthNamespace } from "./auth";
import { setupMainLobbyNamespace } from "./mainLobby";

export function setupNameSpaces(io: Server) {
    const playerManager = new PlayerManager();
    const mainLobbyManager = new MainLobbyManager();

    SetupAuthNamespace(io, playerManager);

    const mainLobby = io.of("/mainlobby");
    setupMainLobbyNamespace(mainLobby, playerManager, mainLobbyManager);
}