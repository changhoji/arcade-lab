import { PlayerState as PlayerBaseState, Position } from './common';

export interface LobbyPlayerState {
  position: Position;
  isMoving: boolean;
}

export type LobbyPlayerSnapshot = PlayerBaseState & LobbyPlayerState;

export interface PlayerMoveData {
  userId: string;
  position: Position;
}

export interface PlayerSkinData {
  userId: string;
  skinIndex: number;
}

export interface PlayerNicknameData {
  userId: string;
  nickname: string;
}

export interface Lobby {
  lobbyId: string;
  name: string;
  currentPlayers: number;
}

export interface CreateRoomRequest {
  gameId: string;
  name: string;
  hostId: string;
}

export interface RoomData {
  roomId: string;
  name: string;
  hostId: string;
  gameId: string;
  currentPlayers: number;
}

export interface RoomPlayerState {
  isReady: boolean;
}

export type RoomPlayerSnapshot = PlayerBaseState & RoomPlayerState;

export interface CreateRoomRequest {
  gameId: string;
  name: string;
}
