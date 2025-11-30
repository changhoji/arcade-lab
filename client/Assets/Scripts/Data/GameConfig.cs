using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ArcadeLab/GameConfig")]
public class GameConfig : ScriptableObject
{
    public string gameId;
    public string displayName;
    public Sprite icon;
    public int maxPlayers;
    public string sceneName;
}
