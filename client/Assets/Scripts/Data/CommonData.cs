using UnityEngine;

namespace ArcadeLab.Data
{
    public class NetworkResult<T>
    {
        public bool success;
        public T data;
        public string error;
    }

    public class Position
    {
        public Position() {}
        public Position(Transform transform)
        {
            this.x = transform.position.x;
            this.y = transform.position.y;
        }

        public float x;
        public float y;
    }

    public class PlayerBaseData
    {
        public string userId;
        public string nickname;
        public int skinIndex;
    }
}
