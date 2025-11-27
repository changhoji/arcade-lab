import { Player, Position } from "./common";

export interface ColorLabPlayerData extends Player {
    position: Position;
    colorIndex: number;
}
