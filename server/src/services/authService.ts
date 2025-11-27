import { Player } from "../types/common";

export class AuthService {
    private players = new Map<string, Player>();
    private socketToUserId = new Map<string, string>();
    private nextKey = 0;

    addPlayer(userId: string, socketId: string): Player {
        const player: Player = {
            userId,
            nickname: `guest${this.nextKey++}`,
            skinIndex: 0
        }
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

    getUser(userId: string)
    {
        return this.players.get(userId);
    }

    getUserIdBySocket(socketId: string) {
        return this.socketToUserId.get(socketId);
    }
}