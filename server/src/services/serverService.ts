import { Lobby } from '../types/lobby';
import { AuthService } from './authService';
import { LobbyService } from './lobbyService';
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

  getLobbyDatas(): Lobby[] {
    const result: Lobby[] = [];
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
    return lobby ? lobby : null;
  }
}
