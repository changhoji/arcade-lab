import { RoomData, RoomPlayerState } from '../types/lobby';
import { AuthService } from './authService';
export class RoomService {
  private roomPlayers = new Map<string, RoomPlayerState>();

  constructor(
    public authService: AuthService,
    public roomId: string,
    public name: string,
    public hostId: string,
    public gameId: string,
    public maxPlayers: number
  ) {}

  toRoomData(): RoomData {
    return {
      roomId: this.roomId,
      name: this.name,
      hostId: this.hostId,
      gameId: this.gameId,
      currentPlayers: this.roomPlayers.size,
      maxPlayers: this.maxPlayers,
    };
  }

  joinRoom(userId: string) {
    this.roomPlayers.set(userId, {
      isReady: false,
    });
  }

  leaveRoom(userId: string) {
    this.roomPlayers.delete(userId);
  }
}
