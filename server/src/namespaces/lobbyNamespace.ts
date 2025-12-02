import { AuthService } from '@/services/authService';
import { LobbyService } from '@/services/lobbyService';
import { RoomService } from '@/services/roomService';
import { ServerService } from '@/services/serverService';
import { NetworkResult, Position } from '@/types/common';
import {
  CreateRoomRequest,
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

      // 여기랑 join이랑 클라이언트 측에서 널체킹하는거 매니저까지 안가게
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
          callback: (result: NetworkResult<RoomData>) => void
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
            data: room.toRoomData(),
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
          callback({
            success: true,
            data: {
              room: roomService.toRoomData(),
              players: roomService.getPlayerSnapshots(),
            },
            error: null,
          });
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
