namespace ArcadeLab.Data
{
    #region Player Datas
    public class LobbyPlayerData : PlayerBaseData
    {
        public Position position;
        public bool isMoving;
    }

    public class PlayerMoveData
    {
        public string userId;
        public Position position;
    }

    public class PlayerSkinData
    {
        public string userId;
        public int skinIndex;
    }

    public class PlayerNicknameData
    {
        public string userId;
        public string nickname;
    }
    #endregion

    public class LobbyData
    {
        public string lobbyId;
        public string name;
        public int currentPlayers;
    }

    public class RoomData
    {
        public string roomId;
        public string name;
        public string hostId;
        public string gameId;
        public int currentPlayers;
        public int maxPlayers;
    }

    public class RoomPlayerData : PlayerBaseData
    {
        public bool isReady;
        public bool isHost;
    }

    public class CreateRoomRequest
    {
        public string gameId;
        public string name;
    }

    public class CreateRoomResponse
    {
        public RoomData room;
        public RoomPlayerData player;
    }

    public class JoinRoomResponse
    {
        public RoomData room;
        public RoomPlayerData[] players;
    }
}
