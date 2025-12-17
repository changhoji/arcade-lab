import { GAME_CONFIGS } from '@/configs/games';
import { ServerService } from '@/services/serverService';
import { ColorLabPlayerSnapshot } from '@/types/colorLab';
import { failure, NetworkResult, Position, success } from '@/types/common';
import { PlayerMovingPayload, PlayerPositionPayload } from '@/types/lobby';
import { Namespace, Server } from 'socket.io';
import { ColorLabService } from '../services/colorLabService';

export class ColorLabNamespace {
  private namespace: Namespace;
  private games = new Map<string, ColorLabService>();

  constructor(
    private io: Server,
    private serverService: ServerService
  ) {
    this.namespace = io.of('/colorLab');
    this.registerHandlers();
  }

  private registerHandlers() {
    this.namespace.on('connection', (socket) => {
      const userId = socket.handshake.auth.userId;
      const roomId = socket.handshake.auth.roomId;

      // create or get game service
      let gameService = this.games.get(roomId);
      if (!gameService) {
        gameService = new ColorLabService(
          this.serverService.getRoomById(roomId)!,
          GAME_CONFIGS['color-lab']
        );
        this.games.set(roomId, gameService);
      }

      socket.join(roomId);

      socket.on(
        'game:init',
        (
          callback: (result: NetworkResult<ColorLabPlayerSnapshot[]>) => void
        ) => {
          if (!gameService) {
            callback(failure('cannot find game service'));
            return;
          }

          console.log('init');
          callback(success(gameService.getPlayerSnapshots()));
        }
      );

      socket.on('game:ready', () => {
        console.log('ready');
        gameService.setPlayerReady(userId);
        if (gameService.isAllReady()) {
          gameService.startCountdown(
            (count) => {
              this.namespace.to(roomId).emit('game:countdown', count);
            },
            () => {
              this.namespace.to(roomId).emit('game:start', {});
            }
          );
        }
      });

      socket.on('player:changePosition', (position: Position) => {
        if (gameService) {
          if (gameService.updatePosition(userId, position)) {
            const payload: PlayerPositionPayload = { userId, position };
            socket.to(roomId).emit('player:positionChanged', payload);
          }
        }
      });

      socket.on('player:changeMoving', (isMoving: boolean) => {
        if (gameService) {
          if (gameService.updateIsMoving(userId, isMoving)) {
            const payload: PlayerMovingPayload = { userId, isMoving };
            socket.to(roomId).emit('player:movingChanged', payload);
          }
        }
      });
    });
  }
}
