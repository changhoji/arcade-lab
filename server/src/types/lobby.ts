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

export interface CreateRoomResponse {
  room: RoomData;
  player: RoomPlayerSnapshot;
}

export interface JoinRoomResponse {
  room: RoomData;
  players: RoomPlayerSnapshot[];
}

export interface PlayerPositionPayload {
  userId: string;
  position: Position;
}

export interface PlayerMovingPayload {
  userId: string;
  isMoving: boolean;
}

export interface PlayerSkinPayload {
  userId: string;
  skinIndex: number;
}

export interface PlayerNicknamePayload {
  userId: string;
  nickname: string;
}

export interface PlayerReadyPayload {
  userId: string;
  isReady: boolean;
}
