import { PlayerState } from '../types/common';

export class AuthService {
  private players = new Map<string, PlayerState>();
  private socketToUserId = new Map<string, string>();
  private nextKey = 0;

  addPlayer(userId: string, socketId: string): PlayerState {
    const player: PlayerState = {
      userId,
      nickname: `guest${this.nextKey++}`,
      skinIndex: 0,
    };
    this.players.set(userId, player);
    this.socketToUserId.set(socketId, userId);
    return player;
  }

  removePlayer(socketId: string) {
    const userId = this.socketToUserId.get(socketId);
    if (userId) {
      this.players.delete(userId);
      this.socketToUserId.delete(socketId);
    }
  }

  getUser(userId: string): PlayerState | null {
    const player = this.players.get(userId);
    return player ? player : null;
  }

  getUserIdBySocket(socketId: string) {
    return this.socketToUserId.get(socketId);
  }

  updateSkinIndex(userId: string, skinIndex: number): boolean {
    const player = this.players.get(userId);
    if (player) {
      player.skinIndex = skinIndex;
      return true;
    }
    return false;
  }

  updateNickname(userId: string, nickname: string): boolean {
    const player = this.players.get(userId);
    if (player) {
      player.nickname = nickname;
      return true;
    }
    return false;
  }
}
