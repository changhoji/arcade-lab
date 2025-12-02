import { PlayerState as PlayerBaseState, Position } from './common';

export interface LobbyPlayerState {
  position: Position;
  isMoving: boolean;
}

export type LobbyPlayerSnapshot = PlayerBaseState & LobbyPlayerState;

export interface Lobby {
  lobbyId: string;
  name: string;
  currentPlayers: number;
}

export interface CreateRoomRequest {
  gameId: string;
  name: string;
}

export interface RoomData {
  roomId: string;
  name: string;
  hostId: string;
  gameId: string;
  currentPlayers: number;
  maxPlayers: number;
}

export interface RoomPlayerState {
  isReady: boolean;
  isHost: boolean;
}

export type RoomPlayerSnapshot = PlayerBaseState & RoomPlayerState;

export interface JoinRoomResponse {
  room: RoomData;
  players: RoomPlayerSnapshot[];
}
