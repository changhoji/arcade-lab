import { Position } from '../types/common';
import { LobbyPlayerSnapshot, LobbyPlayerState } from '../types/lobby';
import { AuthService } from './authService';

export class LobbyService {
    public currentPlayers: number;
    constructor(public authService: AuthService, public lobbyId: string, public name: string) {
        this.currentPlayers = 0;
    }

    private lobbyPlayers = new Map<string, LobbyPlayerState>();
    private nextKey = 0;

    joinLobby(userId: string) {
        this.currentPlayers++;
        this.lobbyPlayers.set(userId, { position: { x: 0, y: 0 }, isMoving: false });
    }

    leaveLobby(userId: string) {
        this.currentPlayers--;
        this.lobbyPlayers.delete(userId);
    }

    getPlayerSnapshots(): LobbyPlayerSnapshot[] {
        const result: LobbyPlayerSnapshot[] = [];
        Array.from(this.lobbyPlayers.entries()).forEach(([userId, player]) => {
            const playerBase = this.authService.getUser(userId);
            if (!playerBase) return;
            result.push({
                ...playerBase,
                ...player
            })
        })
        return result;
    }

    getPlayerSnapshot(userId: string): LobbyPlayerSnapshot {
        return {
            ...this.authService.getUser(userId)!,
            ...this.lobbyPlayers.get(userId)!
        };
    }

    //#region Player Methods
    updatePosition(userId: string, position: Position): boolean {
        const player = this.lobbyPlayers.get(userId);
        if (player) {
            player.position = position;
            return true;
        }
        return false;
    }

    updateIsMoving(userId: string, isMoving: boolean): boolean {
        const player = this.lobbyPlayers.get(userId);
        if (player) {
            player.isMoving = isMoving;
            return true;
        }
        return false;
    }

    getOtherPlayers(excludeUserId: string): Array<{userId: string, player: LobbyPlayerState}> {
        const result: Array<{userId: string, player: LobbyPlayerState}> = [];

        for (const [userId, player] of this.lobbyPlayers.entries()) {
            if (userId !== excludeUserId) {
                result.push({userId, player});
            }
        }

        return result;
    }

    // updateSkin(userId: string, skinIndex: number): boolean {
    //     const player = this.lobbyPlayers.get(userId);
    //     if (player) {
    //         player.skinIndex = skinIndex;
    //         return true;
    //     }
    //     return false;
    // }

    // updateNickname(userId: string, nickname: string): boolean {
    //     const player = this.lobbyPlayers.get(userId);
    //     if (player)
    //     {
    //         player.nickname = nickname;
    //         return true;
    //     }
    //     return false;
    // }

    // getOtherPlayers(userId: string) {
    //     return Array.from(this.lobbyPlayers.values()).filter(p => p.userId !== userId);
    // }
    //#endregion

    //#region Room Methods
    
    //#endregion
}