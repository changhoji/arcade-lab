import { RoomData } from '@/types/lobby';
import { PlayerState as PlayerBaseState, Position } from './common';

export interface ColorLabPlayerState {
  position: Position;
  isMoving: boolean;
  colorIndex: number;
}

export type ColorLabPlayerSnapshot = PlayerBaseState & ColorLabPlayerState;

export interface ColorLabInitResponse {
  room: RoomData;
  players: ColorLabPlayerSnapshot[];
}

export interface TileStepperPayload {
  position: Position;
  stepperId: string | null;
}
