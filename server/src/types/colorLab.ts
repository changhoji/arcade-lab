import { PlayerState, Position } from "./common";

export interface ColorLabPlayerData extends PlayerState {
    position: Position;
    colorIndex: number;
}
