import { Namespace, Server } from 'socket.io';
import { AuthService } from '../services/authService';
import { LobbyService } from '../services/lobbyService';
import { ServerService } from '../services/serverService';
import { Position } from '../types/common';
import { CreateRoomRequest, Lobby, LobbyPlayerSnapshot } from '../types/lobby';
import { generateId } from '../utils/idGenerator';

export class LobbyNamespace {
  private namespace: Namespace;

  constructor(
    private io: Server,
    private serverService: ServerService,
    private authService: AuthService
  ) {
    this.namespace = io.of('/lobby');
    this.registerHandlers();
  }

  private registerHandlers() {
    this.namespace.on('connection', (socket) => {
      // get userId
      let userId: string = '';
      userId = socket.handshake.auth.userId;

      let lobbyService: LobbyService | null = null;

      socket.on('lobby:list', (callback: (lobbies: Lobby[]) => void) => {
        const lobbies = this.serverService.getLobbyDatas();
        callback(lobbies);
      });

      socket.on(
        'lobby:create',
        (name: string, callback: (lobbyId: string | null) => void) => {
          const lobbyId = generateId();
          const lobby = this.serverService.createLobby(lobbyId, name);
          if (lobby) {
            lobby.joinLobby(userId);
            socket.join(lobbyId);
            lobbyService = lobby;
            callback(lobbyId);
          } else {
            callback(null);
          }
        }
      );

      socket.on(
        'lobby:join',
        (lobbyId: string, callback: (lobbyId: string | null) => void) => {
          const lobby = this.serverService.getLobby(lobbyId);
          if (lobby) {
            lobby.joinLobby(userId);
            socket.join(lobbyId);
            lobbyService = lobby;
            callback(lobbyId);
          } else {
            callback(null);
          }
        }
      );

      socket.on(
        'lobby:init',
        (callback: (players: LobbyPlayerSnapshot[]) => void) => {
          if (lobbyService) {
            const players = lobbyService.getPlayerSnapshots();
            callback(players);

            socket
              .to(lobbyService.lobbyId)
              .emit('player:joined', lobbyService.getPlayerSnapshot(userId));
          }
        }
      );

      socket.on('player:changePosition', (position: Position) => {
        if (lobbyService) {
          if (lobbyService.updatePosition(userId, position)) {
            console.log(`emit ${userId}`);
            socket
              .to(lobbyService.lobbyId)
              .emit('player:positionChanged', userId, position);
          }
        }
      });

      socket.on('player:changeIsMoving', (isMoving: boolean) => {
        if (lobbyService) {
          if (lobbyService.updateIsMoving(userId, isMoving)) {
            socket
              .to(lobbyService.lobbyId)
              .emit('player:isMovingChanged', userId, isMoving);
          }
        }
      });

      socket.on('player:changeSkin', (skinIndex: number) => {
        if (this.authService && lobbyService) {
          if (this.authService.updateSkinIndex(userId, skinIndex)) {
            socket
              .to(lobbyService.lobbyId)
              .emit('player:skinChanged', userId, skinIndex);
          }
        }
      });

      socket.on('player:changeNickname', (nickname: string) => {
        if (this.authService && lobbyService) {
          if (this.authService.updateNickname(userId, nickname)) {
            socket
              .to(lobbyService.lobbyId)
              .emit('player:nicknameChanged', userId, nickname);
          }
        }
      });

      socket.on('room:create', (request: CreateRoomRequest) => {});

      socket.on('disconnecting', () => {
        if (lobbyService) {
          lobbyService.leaveLobby(userId);
          socket.to(lobbyService.lobbyId).emit('player:left', userId);
          if (lobbyService.currentPlayers === 0) {
            console.log(`lobby ${lobbyService} removed`);
            this.serverService.removeLobby(lobbyService.lobbyId);
          }
        }
      });
    });
  }
}
