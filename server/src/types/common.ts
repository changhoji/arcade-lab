export interface NetworkResult<T> {
  success: boolean;
  data: T | null;
  error: string | null;
}

export function success<T>(data: T): NetworkResult<T> {
  return {
    success: true,
    data,
    error: null,
  };
}

export function failure<T = never>(error: string): NetworkResult<T> {
  return {
    success: false,
    data: null,
    error,
  };
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
