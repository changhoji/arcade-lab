import { Namespace, Server } from 'socket.io';
import { AuthService } from '../services/authService';
import { LobbyService } from '../services/lobbyService';
import { RoomService } from '../services/roomService';
import { ServerService } from '../services/serverService';
import { Position } from '../types/common';
import {
  CreateRoomRequest,
  Lobby as LobbyData,
  LobbyPlayerSnapshot,
  RoomData,
} from '../types/lobby';
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

      let lobbyService: LobbyService | null;
      let roomService: RoomService | null;

      socket.on('lobby:list', (callback: (lobbies: LobbyData[]) => void) => {
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

      socket.on(
        'room:list',
        (gameId: string, callback: (rooms: RoomData[]) => void) => {
          console.log('room list');
          if (lobbyService) {
            console.log('room list');
            const rooms = lobbyService.getRoomDatas();
            console.log(rooms);
            callback(rooms);
          }
        }
      );

      socket.on(
        'room:create',
        (
          request: CreateRoomRequest,
          callback: (roomId: string | null) => void
        ) => {
          console.log('create request');
          if (!lobbyService) {
            callback(null);
            return;
          }

          const roomId = generateId();
          const room = lobbyService.createRoom(roomId, userId, request);
          if (!room) {
            callback(null);
            return;
          }

          console.log('create success');
          room.joinRoom(userId);
          socket.join(`room:${roomId}`);
          roomService = room;
          callback(roomId);
        }
      );

      socket.on(
        'room:join',
        (roomId: string, callback: (roomId: string | null) => void) => {
          if (!lobbyService) {
            callback(null);
            return;
          }

          const room = lobbyService.joinRoom(roomId);
          if (!room) {
            callback(null);
            return;
          }

          room.joinRoom(userId);
          socket.join(`room:${roomId}`);
          roomService = room;
          callback(roomId);
        }
      );

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
