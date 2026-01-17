import { GameConfig } from '@/configs/games';
import { AuthService } from '@/services/authService';
import { RoomService } from '@/services/roomService';
import { PlayerState } from '@/types/common';
import { RoomPlayerSnapshot } from '../types/lobby';

export abstract class GameService<TPlayerState> {
  protected players = new Map<string, TPlayerState>();
  protected readyPlayers = new Set<string>();
  protected authService: AuthService;

  private gameTimer?: NodeJS.Timeout;
  private remainingTime = 60;

  constructor(
    protected roomService: RoomService,
    protected config: GameConfig
  ) {
    if (roomService === null) {
      console.log('room is null');
    }
    this.authService = AuthService.getInstance();
    for (const player of roomService.getPlayerSnapshots()) {
      const gamePlayer = this.createPlayerState(player);
      this.players.set(player.userId, gamePlayer);
    }
  }

  get maxPlayers(): number {
    return this.config.maxPlayers;
  }

  protected abstract createPlayerState(
    player: RoomPlayerSnapshot
  ): TPlayerState;

  setPlayerReady(userId: string) {
    this.readyPlayers.add(userId);
  }

  getPlayerSnapshots(): (TPlayerState & PlayerState)[] {
    const result: (TPlayerState & PlayerState)[] = [];
    Array.from(this.players.entries()).forEach(([userId, player]) => {
      result.push({
        ...this.authService.getUser(userId)!,
        ...player,
      });
    });
    return result;
  }

  getPlayerSnapshot(userId: string): TPlayerState & PlayerState {
    return {
      ...this.authService.getUser(userId)!,
      ...this.players.get(userId)!,
    };
  }

  isAllReady(): boolean {
    return this.readyPlayers.size === this.maxPlayers;
  }

  // countdown for starting game
  startCountdown(onTick: (count: number) => void, onComplete: () => void) {
    let count = 3;
    const interval = setInterval(() => {
      if (count > 0) {
        onTick(count);
        count--;
      } else {
        clearInterval(interval);
        onComplete();
      }
    }, 1000);
  }

  // call to start game
  startGame(duration: number) {
    this.remainingTime = duration;

    this.gameTimer = setInterval(() => {
      this.remainingTime--;

      this.onTimeTick?.(this.remainingTime);

      if (this.remainingTime <= 0) {
        this.endGame();
      }
    }, 1000);
  }

  // when game timer is done
  endGame() {
    if (this.gameTimer) {
      clearInterval(this.gameTimer);
    }
    this.onGameEnd?.();
  }

  // event logic for each game service
  onTimeTick?: (remainingTime: number) => void;
  onGameEnd?: () => void;
}
