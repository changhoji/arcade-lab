import { Server } from 'socket.io';
import { AuthService } from '../services/authService';
import { ServerService } from '../services/serverService';
import { AuthNamespace } from './authNamespace';
import { LobbyNamespace } from './lobbyNamespace';

export function setupNameSpaces(io: Server) {
  const authService = new AuthService();
  const serverService = new ServerService(authService);

  new AuthNamespace(io, serverService, authService);

  serverService.createLobby('1', 'test1');
  serverService.createLobby('2', 'test2');

  new LobbyNamespace(io, serverService, authService);
}
