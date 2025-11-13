import { Position } from '../types/common';
import { LobbyPlayer } from '../types/lobby';
export class PlayerManager {
    private players = new Map<string, LobbyPlayer>();
    private socketToUserId = new Map<string, string>();

    addDummyPlayers(count: number = 3) {
        for (let i = 0; i < count; i++) {
            const dummyUserId = `dummy-${i}`;
            const dummySocketId = `socket-dummy-${i}`;
            const dummyPlayer: LobbyPlayer = {
                userId: dummyUserId,
                socketId: dummySocketId,
                position: {
                    x: Math.random() * 10 - 5,  // -5 ~ 5 랜덤
                    y: Math.random() * 10 - 5   // -5 ~ 5 랜덤
                }
            };
            this.players.set(dummyUserId, dummyPlayer);
            this.socketToUserId.set(dummySocketId, dummyUserId);
        }
        console.log(`Added ${count} dummy players`);
    }

    addPlayer(userId: string, socketId: string) {
        const player: LobbyPlayer = {
            userId,
            socketId,
            position: {x: 0, y: 0}
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
        return userId;
    }

    updatePosition(userId: string, position: Position) {
        const player = this.players.get(userId);
        if (player) {
            player.position = position;
            return true;
        }
        return false;
    }

    getOtherPlayers(userId: string)
    {
        return Array.from(this.players.values()).filter(p => p.userId !== userId);
    }

    getUserIdBySocket(socketId: string)
    {
        return this.socketToUserId.get(socketId);
    }
}