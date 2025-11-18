import { PlayerBase, Position } from './common';

export interface LobbyPlayerData extends PlayerBase {
    position: Position;
    skinIndex: number;
}

export interface PlayerMoveData {
    userId: string;
    position: Position
}

export interface PlayerSkinData {
    userId: string;
    skinIndex: number;
}
