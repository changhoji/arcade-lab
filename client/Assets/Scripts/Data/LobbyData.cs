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

    #region Room Datas
    public class RoomData
    {
        public string roomId;
        public string gameId;
        public string roomName;
        public string hostUserId;
        public int currentPlayers;
        public int maxPlayers;
    }

    public class RoomPlayerData : PlayerBaseData
    {
        public bool isReady;
        public bool isHost;
    }

    public class RoomCreateData
    {
        public string gameId;
        public string roomName;
        public int maxPlayers;
    }
    #endregion
}
