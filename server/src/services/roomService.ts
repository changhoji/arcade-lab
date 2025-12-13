import { AuthService } from '@/services/authService';
import { RoomData, RoomPlayerSnapshot, RoomPlayerState } from '@/types/lobby';
export class RoomService {
  public roomPlayers = new Map<string, RoomPlayerState>();

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
      isReady: userId === this.hostId,
      isHost: userId === this.hostId,
    });
  }

  leaveRoom(userId: string) {
    this.roomPlayers.delete(userId);
  }

  start(): boolean {
    let allReady = true;
    this.roomPlayers.forEach((player) => {
      if (!player.isReady) {
        allReady = false;
      }
    });
    return allReady;
  }

  getPlayerSnapshots(): RoomPlayerSnapshot[] {
    const result: RoomPlayerSnapshot[] = [];
    Array.from(this.roomPlayers.entries()).forEach(([userId, player]) => {
      const playerBase = this.authService.getUser(userId);
      if (!playerBase) return;
      result.push({
        ...playerBase,
        ...player,
      });
    });
    return result;
  }

  getPlayerSnapshot(userId: string): RoomPlayerSnapshot {
    return {
      ...this.authService.getUser(userId)!,
      ...this.roomPlayers.get(userId)!,
    };
  }

  updateReady(userId: string, isReady: boolean): boolean {
    const player = this.roomPlayers.get(userId);
    if (player) {
      player.isReady = isReady;
      return true;
    }
    return false;
  }
}
