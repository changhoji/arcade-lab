namespace ArcadeLab.Data
{
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
}
