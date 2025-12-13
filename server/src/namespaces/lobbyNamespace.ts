import { AuthService } from '@/services/authService';
import { LobbyService } from '@/services/lobbyService';
import { RoomService } from '@/services/roomService';
import { ServerService } from '@/services/serverService';
import { NetworkResult, Position } from '@/types/common';
import {
  CreateRoomRequest,
  CreateRoomResponse,
  JoinRoomResponse,
  Lobby as LobbyData,
  LobbyPlayerSnapshot,
  RoomData,
} from '@/types/lobby';
import { generateId } from '@/utils/idGenerator';
import { Namespace, Server } from 'socket.io';

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

      socket.on(
        'lobby:list',
        (callback: (result: NetworkResult<LobbyData[]>) => void) => {
          const lobbies = this.serverService.getLobbyDatas();
          callback({
            success: true,
            data: lobbies,
            error: null,
          });
        }
      );

      socket.on(
        'lobby:create',
        (name: string, callback: (result: NetworkResult<string>) => void) => {
          const lobbyId = generateId();
          const lobby = this.serverService.createLobby(lobbyId, name);
          if (!lobby) {
            callback({
              success: false,
              data: null,
              error: '',
            });
            return;
          }

          lobby.joinLobby(userId);
          socket.join(lobbyId);
          lobbyService = lobby;
          callback({
            success: true,
            data: lobbyId,
            error: null,
          });
        }
      );

      socket.on(
        'lobby:join',
        (
          lobbyId: string,
          callback: (result: NetworkResult<string>) => void
        ) => {
          const lobby = this.serverService.getLobby(lobbyId);
          if (!lobby) {
            callback({
              success: false,
              data: null,
              error: 'cannot find lobby',
            });
            return;
          }

          lobby.joinLobby(userId);
          socket.join(lobbyId);
          lobbyService = lobby;
          callback({
            success: true,
            data: lobbyId,
            error: null,
          });
        }
      );

      socket.on(
        'lobby:init',
        (callback: (result: NetworkResult<LobbyPlayerSnapshot[]>) => void) => {
          if (!lobbyService) {
            callback({
              success: false,
              data: null,
              error: 'Cannot find lobby',
            });
            return;
          }

          const players = lobbyService.getPlayerSnapshots();
          callback({
            success: true,
            data: players,
            error: null,
          });

          socket
            .to(lobbyService.lobbyId)
            .emit('player:joined', lobbyService.getPlayerSnapshot(userId));
        }
      );

      socket.on('player:changePosition', (position: Position) => {
        if (lobbyService) {
          if (lobbyService.updatePosition(userId, position)) {
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

      socket.on('player:changeReady', (isReady: boolean) => {
        console.log('got changeReady');
        if (roomService) {
          if (roomService.updateReady(userId, isReady)) {
            console.log('update success');
            socket
              .to(`room:${roomService.roomId}`)
              .emit('player:readyChanged', userId, isReady);
            return;
          }
          console.log('update failed');
        }
      });

      socket.on(
        'room:list',
        (
          gameId: string,
          callback: (result: NetworkResult<RoomData[]>) => void
        ) => {
          console.log('room list');
          if (!lobbyService) {
            callback({
              success: false,
              data: null,
              error: 'cannot find lobby',
            });
            return;
          }

          const rooms = lobbyService.getRoomDatas();
          callback({
            success: true,
            data: rooms,
            error: null,
          });
        }
      );

      socket.on(
        'room:create',
        (
          request: CreateRoomRequest,
          callback: (result: NetworkResult<CreateRoomResponse>) => void
        ) => {
          if (!lobbyService) {
            callback({
              success: false,
              data: null,
              error: 'cannot find joined lobby',
            });
            return;
          }

          const roomId = generateId();
          const room = lobbyService.createRoom(roomId, userId, request);
          if (!room) {
            callback({
              success: false,
              data: null,
              error: 'duplicated room id',
            });
            return;
          }

          console.log('create success');
          room.joinRoom(userId);
          socket.join(`room:${roomId}`);
          roomService = room;
          callback({
            success: true,
            data: {
              room: room.toRoomData(),
              player: roomService.getPlayerSnapshot(userId),
            },
            error: null,
          });
        }
      );

      socket.on(
        'room:join',
        (
          roomId: string,
          callback: (result: NetworkResult<JoinRoomResponse>) => void
        ) => {
          if (!lobbyService) {
            callback({
              success: false,
              data: null,
              error: 'cannot find joined lobby',
            });
            return;
          }

          const room = lobbyService.joinRoom(roomId);
          if (!room) {
            callback({
              success: false,
              data: null,
              error: 'cannot find joined room',
            });
            return;
          }

          room.joinRoom(userId);
          socket.join(`room:${roomId}`);
          roomService = room;
          const player = roomService.getPlayerSnapshot(userId);
          console.log(roomService.getPlayerSnapshots());
          callback({
            success: true,
            data: {
              room: roomService.toRoomData(),
              players: roomService.getPlayerSnapshots(),
            },
            error: null,
          });
          socket.to(`room:${roomId}`).emit('room:joined', player);
        }
      );

      socket.on(
        'room:leave',
        (callback: (result: NetworkResult<void>) => void) => {
          if (!lobbyService) {
            callback({
              success: false,
              data: null,
              error: 'cannot find lobby',
            });
            return;
          }

          if (!roomService) {
            callback({
              success: false,
              data: null,
              error: 'cannot find room',
            });
            return;
          }

          roomService.leaveRoom(userId);
          callback({
            success: true,
            data: null,
            error: null,
          });

          socket.to(`room:${roomService.roomId}`).emit('room:left', userId);
          socket.leave(`room:${roomService.roomId}`);
          if (roomService.roomPlayers.size === 0) {
            lobbyService.removeRoom(roomService.roomId);
          }
          roomService = null;
        }
      );

      socket.on(
        'room:start',
        (callback: (result: NetworkResult<void>) => void) => {
          if (!lobbyService) {
            callback({
              success: false,
              data: null,
              error: 'cannot find lobby',
            });
            return;
          }

          if (!roomService) {
            callback({
              success: false,
              data: null,
              error: 'cannot find room',
            });
            return;
          }

          if (!roomService.start()) {
            callback({
              success: false,
              data: null,
              error: 'not all player is ready',
            });
            return;
          }

          callback({
            success: true,
            data: null,
            error: null,
          });
          socket.to(`room:${roomService.roomId}`).emit('room:started');
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
