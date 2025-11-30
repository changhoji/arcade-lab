import { RoomPlayerData } from '../types/lobby';
import { AuthService } from './authService';
export class RoomService {
  constructor(
    public authService: AuthService,
    public roomId: string,
    public name: string,
    public hostId: string,
    public gameId: string
  ) {}

  private roomPlayers = new Map<string, RoomPlayerData>();
}
