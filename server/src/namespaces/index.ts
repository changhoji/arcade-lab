import { AuthNamespace } from '@/namespaces/authNamespace';
import { ColorLabNamespace } from '@/namespaces/colorLabNamespace';
import { LobbyNamespace } from '@/namespaces/lobbyNamespace';
import { AuthService } from '@/services/authService';
import { ServerService } from '@/services/serverService';
import { Server } from 'socket.io';

export function setupNameSpaces(io: Server) {
  const authService = new AuthService();
  AuthService.setInstance(authService);
  const serverService = new ServerService(authService);

  new AuthNamespace(io, serverService, authService);

  serverService.createLobby('1', 'test1');
  serverService.createLobby('2', 'test2');

  new LobbyNamespace(io, serverService, authService);
  new ColorLabNamespace(io, serverService);
}
