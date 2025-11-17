import { PlayerBase, Position } from "./common";

export interface LobbyPlayer extends PlayerBase {
    position: Position;
    skinIndex: number;
}

export interface LobbyState {
    lobbyPlayers: LobbyPlayer[];
}

export interface SkinData {
    userId: string;
    skinIndex: number;
}