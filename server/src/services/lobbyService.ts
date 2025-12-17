import { GAME_CONFIGS } from '@/configs/games';
import { AuthService } from '@/services/authService';
import { RoomService } from '@/services/roomService';
import { Position } from '@/types/common';
import {
  CreateRoomRequest,
  LobbyPlayerSnapshot,
  LobbyPlayerState,
  RoomData,
} from '@/types/lobby';

export class LobbyService {
  public currentPlayers: number;

  private lobbyPlayers = new Map<string, LobbyPlayerState>();
  private rooms = new Map<string, RoomService>();

  constructor(
    public authService: AuthService,
    public lobbyId: string,
    public name: string
  ) {
    this.currentPlayers = 0;
  }

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

  createRoom(
    roomId: string,
    hostId: string,
    request: CreateRoomRequest
  ): RoomService | null {
    if (this.rooms.has(roomId)) {
      return null;
    }
    const room = new RoomService(
      this.authService,
      roomId,
      request.name,
      hostId,
      request.gameId,
      GAME_CONFIGS[request.gameId].maxPlayers
    );
    this.rooms.set(roomId, room);
    return room;
  }

  getRoomById(roomId: string): RoomService | null {
    const room = this.rooms.get(roomId);
    console.log(`in lobbyservice, ${room}`);
    return room ?? null;
  }

  removeRoom(roomId: string) {
    this.rooms.delete(roomId);
  }

  joinRoom(roomId: string): RoomService | null {
    const room = this.rooms.get(roomId);
    return room ?? null;
  }

  getRoomDatas(): RoomData[] {
    const result: RoomData[] = [];
    Array.from(this.rooms.values()).forEach((roomService) => {
      result.push(roomService.toRoomData());
    });
    return result;
  }
}
