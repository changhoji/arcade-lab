import { PlayerState } from '@/types/common';

export class AuthService {
  private static instance: AuthService;

  private players = new Map<string, PlayerState>();
  private nextKey = 0;

  constructor() {}

  static setInstance(authService: AuthService) {
    this.instance = authService;
  }
  static getInstance() {
    return this.instance;
  }

  addPlayer(userId: string): PlayerState | null {
    if (this.players.has(userId)) {
      return null;
    }
    const player: PlayerState = {
      userId,
      nickname: `guest${this.nextKey++}`,
      skinIndex: 0,
    };
    this.players.set(userId, player);
    return player;
  }

  removePlayer(userId: string) {
    this.players.delete(userId);
  }

  getUser(userId: string): PlayerState | null {
    const player = this.players.get(userId);
    return player ? player : null;
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
