export interface GameConfig {
  gameId: string;
  name: string;
  maxPlayers: number;
}

export const GAME_CONFIGS: Record<string, GameConfig> = {
  'color-lab': { gameId: 'color-lab', name: 'Color Lab', maxPlayers: 2 },
};
