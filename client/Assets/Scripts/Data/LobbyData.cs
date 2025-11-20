namespace ArcadeLab.Data
{
    #region Player Datas
    public class LobbyPlayerData : PlayerBase
    {
        public Position position;
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

    public class RoomPlayerData : PlayerBase
    {
        public bool isReady;
        public bool isHost;
    }

    public class CreateRoomRequest
    {
        public CreateRoomRequest(string gameId, string roomName, int maxPlayers)
        {
            this.gameId = gameId;
            this.roomName = roomName;
            this.maxPlayers = maxPlayers;
        }

        public string gameId;
        public string roomName;
        public int maxPlayers;
    }
    #endregion
}
