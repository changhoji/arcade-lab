import { v4 as uuidv4 } from 'uuid';
import { CreateRoomRequest, RoomData } from '../types/lobby';
export class RoomService {
    private rooms = new Map<string, RoomData>();

    createRoom(hostUserId: string, createRequest: CreateRoomRequest): RoomData {
        const roomId = uuidv4();
        const roomData: RoomData = {
            roomId,
            gameId: createRequest.gameId,
            roomName:createRequest.roomName,
            hostUserId,
            currentPlayers: 1,
            maxPlayers: createRequest.maxPlayers
        }
        this.rooms.set(roomId, roomData);
        return roomData;
    }

    removeRoom(roomId: string) {
        this.rooms.delete(roomId);
    }

    joinRoom(roomId: string, userId: string) {
        // this.rooms.get(roomId).
    }
}