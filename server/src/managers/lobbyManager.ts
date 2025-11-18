import { Position } from '../types/common';
import { LobbyPlayerData } from '../types/lobby';

export class LobbyManager {
    private lobbyPlayers = new Map<string, LobbyPlayerData>();
    private nextKey = 0;

    addPlayer(userId: string): LobbyPlayerData {
        const player: LobbyPlayerData = {
            userId,
            skinIndex: 0,
            position: { x: 0, y: 0 },
            nickname: `guest${this.nextKey++}`,
        };
        this.lobbyPlayers.set(userId, player);
        return player;
    }

    removePlayer(userId: string) {
        this.lobbyPlayers.delete(userId);
    }

    updatePosition(userId: string, position: Position): boolean {
        const player = this.lobbyPlayers.get(userId);
        if (player) {
            player.position = position;
            return true;
        }
        return false;
    }

    updateSkin(userId: string, skinIndex: number): boolean {
        const player = this.lobbyPlayers.get(userId);
        if (player) {
            player.skinIndex = skinIndex;
            return true;
        }
        return false;
    }

    updateNickname(userId: string, nickname: string): boolean {
        const player = this.lobbyPlayers.get(userId);
        if (player)
        {
            player.nickname = nickname;
            return true;
        }
        return false;
    }

    getOtherPlayers(userId: string) {
        return Array.from(this.lobbyPlayers.values()).filter(p => p.userId !== userId);
    }
}