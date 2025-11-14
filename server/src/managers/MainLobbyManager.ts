import { Position } from '../types/common';
import { LobbyPlayer } from '../types/lobby';

export class MainLobbyManager {
    private lobbyPlayers = new Map<string, LobbyPlayer>();

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
            this.lobbyPlayers.set(dummyUserId, dummyPlayer);
        }
        console.log(`Added ${count} dummy players`);
    }

    addPlayer(userId: string, socketId: string) {
        const player: LobbyPlayer = {
            userId,
            socketId,
            position: { x: 0, y: 0 }
        };
        this.lobbyPlayers.set(userId, player);
        return player;
    }

    removePlayer(userId: string) {
        this.lobbyPlayers.delete(userId);
    }

    updatePosition(userId: string, position: Position) {
        const player = this.lobbyPlayers.get(userId);
        if (player) {
            player.position = position;
            return true;
        }
        return false;
    }

    getOtherPlayers(userId: string) {
        return Array.from(this.lobbyPlayers.values()).filter(p => p.userId !== userId);
    }
}