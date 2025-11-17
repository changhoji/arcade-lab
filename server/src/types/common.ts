export interface Position {
    x: number;
    y: number;
}

export interface PlayerBase {
    userId: string;
    socketId: string;
    skinIndex: number;
    nickname?: string;
}
