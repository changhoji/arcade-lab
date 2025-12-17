import { GameService } from '@/services/gameService';
import { ColorLabPlayerState } from '@/types/colorLab';
import { Position } from '@/types/common';
import { RoomPlayerSnapshot } from '@/types/lobby';

export class ColorLabService extends GameService<ColorLabPlayerState> {
  protected createPlayerState(player: RoomPlayerSnapshot): ColorLabPlayerState {
    return player.isHost
      ? { position: { x: -5, y: 0 }, isMoving: false, colorIndex: 0 }
      : { position: { x: 5, y: 0 }, isMoving: false, colorIndex: 1 };
  }

  updatePosition(userId: string, position: Position): boolean {
    const player = this.players.get(userId);
    if (player) {
      player.position = position;
      return true;
    }
    return false;
  }

  updateIsMoving(userId: string, isMoving: boolean): boolean {
    const player = this.players.get(userId);
    if (player) {
      player.isMoving = isMoving;
      return true;
    }
    return false;
  }
}
