using UnityEngine;

namespace ArcadeLab.Data
{
    public class ColorLabPlayerData : PlayerBaseData
    {
        public Position position;
        public bool isMoving;
        public int colorIndex;
    }

    public class ColorLabInitResponse
    {
        public RoomData room;
        public ColorLabPlayerData[] players;
    }

    public class TileStepperPayload
    {
        public Vector2Int position;
        public string stepperId;
    }
}