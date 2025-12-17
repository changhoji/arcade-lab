import { AuthService } from '@/services/authService';
import { LobbyService } from '@/services/lobbyService';
import { RoomService } from '@/services/roomService';
import { Lobby as LobbyData } from '@/types/lobby';
export class ServerService {
  private lobbies: Map<string, LobbyService> = new Map();

  constructor(public authService: AuthService) {}

  createLobby(lobbyId: string, name: string): LobbyService | null {
    if (this.lobbies.has(lobbyId)) {
      return null;
    }
    const lobby = new LobbyService(this.authService, lobbyId, name);
    this.lobbies.set(lobbyId, lobby);
    console.log('lobby created!');
    return lobby;
  }

  removeLobby(lobbyId: string) {
    if (this.lobbies.has(lobbyId)) {
      this.lobbies.delete(lobbyId);
    }
  }

  getLobbyDatas(): LobbyData[] {
    const result: LobbyData[] = [];
    Array.from(this.lobbies.values()).forEach((lobbyService) => {
      result.push({
        lobbyId: lobbyService.lobbyId,
        name: lobbyService.name,
        currentPlayers: lobbyService.currentPlayers,
      });
    });
    return result;
  }

  getLobby(lobbyId: string): LobbyService | null {
    const lobby = this.lobbies.get(lobbyId);
    return lobby ?? null;
  }

  getRoomById(roomId: string): RoomService | null {
    for (const lobby of this.lobbies.values()) {
      const room = lobby.getRoomById(roomId);
      if (room) return room;
    }
    return null;
  }
}
