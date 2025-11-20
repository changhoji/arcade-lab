using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ArcadeLab/GameConfig")]
public class GameConfig : ScriptableObject
{
    public string gmaeId;

    public string displayName;
    public Sprite icon;
    public int maxPlayers;
    public string sceneName;
}
