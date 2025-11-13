import { PlayerBase, Position } from "./common";

export interface LobbyPlayer extends PlayerBase {
    position: Position;
}

export interface LobbyState {
    lobbyPlayers: LobbyPlayer[];
}