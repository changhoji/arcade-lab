import { Position } from '../types/common';
import { LobbyPlayer } from '../types/lobby';

export class LobbyService {
    public currentPlayers: number;
    constructor(public lobbyId: string, public name: string) {
        this.currentPlayers = 0;
    }

    private lobbyPlayers = new Map<string, LobbyPlayer>();
    private nextKey = 0;

    joinLobby(userId: string) {
        this.currentPlayers++;
        this.lobbyPlayers.set(userId, { position: { x: 0, y: 0 }});
    }

    leaveLobby(userId: string) {
        this.currentPlayers--;
    }

    //#region Player Methods
    addLobbyPlayer(userId: string): LobbyPlayer {
        const player: LobbyPlayer = {
            position: { x: 0, y: 0 },
        };
        this.lobbyPlayers.set(userId, player);
        return player;
    }

    updatePosition(userId: string, position: Position): boolean {
        const player = this.lobbyPlayers.get(userId);
        if (player) {
            player.position = position;
            return true;
        }
        return false;
    }

    getOtherPlayers(excludeUserId: string): Array<{userId: string, player: LobbyPlayer}> {
        const result: Array<{userId: string, player: LobbyPlayer}> = [];

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