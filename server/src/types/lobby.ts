import { PlayerState, Position } from './common';

export interface LobbyPlayerState {
  position: Position;
  isMoving: boolean;
}

export type LobbyPlayerSnapshot = PlayerState & LobbyPlayerState;

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
  roomName: string;
  hostId: string;
}

//#region Room Types
export interface RoomData {
  roomId: string;
  gameId: string;
  roomName: string;
  hostUserId: string;
  currentPlayers: number;
  maxPlayers: number;
}

export interface RoomPlayerData {
  isReady: boolean;
  isHost: boolean;
}

export interface CreateRoomRequest {
  gameId: string;
  roomName: string;
  maxPlayers: number;
}
//#endregion
