import { PlayerBase, Position } from "./common";

export interface ColorLabPlayerData extends PlayerBase {
    position: Position;
    colorIndex: number;
}
