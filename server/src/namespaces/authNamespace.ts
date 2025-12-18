import { AuthService } from '@/services/authService';
import { ServerService } from '@/services/serverService';
import { NetworkResult, PlayerState } from '@/types/common';
import { generateId } from '@/utils/idGenerator';
import { Namespace, Server } from 'socket.io';

export class AuthNamespace {
  private namespace: Namespace;
  constructor(
    private io: Server,
    private serverService: ServerService,
    private authService: AuthService
  ) {
    this.namespace = io.of('/auth');
    this.registerHandlers();
  }

  private registerHandlers() {
    this.namespace.on('connection', (socket) => {
      let userId: string | null;
      // guest signin request
      socket.on(
        'auth:guest',
        (callback: (result: NetworkResult<PlayerState>) => void) => {
          userId = generateId();
          const player = this.authService.addPlayer(userId);
          if (!player) {
            callback({
              success: false,
              data: null,
              error: 'duplicated user id',
            });
            return;
          }

          console.log(`Guest signin: ${userId}`);
          callback({
            success: true,
            data: player,
            error: null,
          });
        }
      );

      // client disconnected
      socket.on('disconnect', () => {
        if (userId) {
          this.authService.removePlayer(userId);
        }
        console.log(`player ${userId} disconnected from auth`);
      });
    });
  }
}
