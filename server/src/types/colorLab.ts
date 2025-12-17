import { PlayerState as PlayerBaseState, Position } from './common';

export interface ColorLabPlayerState {
  position: Position;
  isMoving: boolean;
  colorIndex: number;
}

export type ColorLabPlayerSnapshot = PlayerBaseState & ColorLabPlayerState;
