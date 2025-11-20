import { PlayerBase, Position } from './common';

//#region Player Types
export interface LobbyPlayerData extends PlayerBase {
    position: Position;
}

export interface PlayerMoveData {
    userId: string;
    position: Position
}

export interface PlayerSkinData {
    userId: string;
    skinIndex: number;
}

export interface PlayerNicknameData {
    userId: string;
    nickname: string;
}
//#endregion

//#region Room Types
export interface RoomData {
    roomId: string;
    gameId: string;
    roomName: string;
    hostUserId: string;
    currentPlayers: number;
    maxPlayers: number;
}

export interface RoomPlayerData extends PlayerBase {
    isReady: boolean;
    isHost: boolean;
}

export interface CreateRoomRequest {
    gameId: string;
    roomName: string;
    maxPlayers: number;
}
//#endregion

