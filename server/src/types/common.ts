export interface NetworkResult<T> {
  success: boolean;
  data: T | null;
  error: string | null;
}

export interface Position {
  x: number;
  y: number;
}

export interface PlayerState {
  userId: string;
  nickname: string;
  skinIndex: number;
}
