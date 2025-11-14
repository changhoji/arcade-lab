import { PlayerBase } from "../types/common";

interface PlayerInfo extends PlayerBase {
    namespace: string;
}

export class PlayerManager {
    private players = new Map<string, PlayerInfo>();
    private socketToUserId = new Map<string, string>();

    addPlayer(userId: string, socketId: string, namespace: string = "/") {
        const player: PlayerInfo = { userId, socketId, namespace };
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

    updateNamespace(userId: string, namespace: string) {
        const player = this.players.get(userId);
        if (player) {
            player.namespace = namespace;
            return true;
        }
        return false;
    }

    getUserIdBySocket(socketId: string) {
        return this.socketToUserId.get(socketId);
    }
}