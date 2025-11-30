import { Position } from '../types/common';
import { LobbyPlayerSnapshot, LobbyPlayerState } from '../types/lobby';
import { AuthService } from './authService';
import { RoomService } from './roomService';

export class LobbyService {
  public currentPlayers: number;

  constructor(
    public authService: AuthService,
    public lobbyId: string,
    public name: string
  ) {
    this.currentPlayers = 0;
  }

  private lobbyPlayers = new Map<string, LobbyPlayerState>();
  private rooms = new Map<string, RoomService>();

  joinLobby(userId: string) {
    this.currentPlayers++;
    this.lobbyPlayers.set(userId, {
      position: { x: 0, y: 0 },
      isMoving: false,
    });
  }

  leaveLobby(userId: string) {
    this.currentPlayers--;
    this.lobbyPlayers.delete(userId);
  }

  getPlayerSnapshots(): LobbyPlayerSnapshot[] {
    const result: LobbyPlayerSnapshot[] = [];
    Array.from(this.lobbyPlayers.entries()).forEach(([userId, player]) => {
      const playerBase = this.authService.getUser(userId);
      if (!playerBase) return;
      result.push({
        ...playerBase,
        ...player,
      });
    });
    return result;
  }

  getPlayerSnapshot(userId: string): LobbyPlayerSnapshot {
    return {
      ...this.authService.getUser(userId)!,
      ...this.lobbyPlayers.get(userId)!,
    };
  }

  updatePosition(userId: string, position: Position): boolean {
    const player = this.lobbyPlayers.get(userId);
    if (player) {
      player.position = position;
      return true;
    }
    return false;
  }

  updateIsMoving(userId: string, isMoving: boolean): boolean {
    const player = this.lobbyPlayers.get(userId);
    if (player) {
      player.isMoving = isMoving;
      return true;
    }
    return false;
  }

  getOtherPlayers(
    excludeUserId: string
  ): Array<{ userId: string; player: LobbyPlayerState }> {
    const result: Array<{ userId: string; player: LobbyPlayerState }> = [];

    for (const [userId, player] of this.lobbyPlayers.entries()) {
      if (userId !== excludeUserId) {
        result.push({ userId, player });
      }
    }

    return result;
  }

  createRoom(roomId: string, name: string, gameId: string): RoomService | null {
    if (this.rooms.has(roomId)) {
      return null;
    }
    // const room = new RoomService()
    return null;
  }
}
