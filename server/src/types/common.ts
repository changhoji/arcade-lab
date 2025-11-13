export interface Position {
    x: number;
    y: number;
}

export interface PlayerBase {
    userId: string;
    socketId: string;
    nickname?: string;
}
