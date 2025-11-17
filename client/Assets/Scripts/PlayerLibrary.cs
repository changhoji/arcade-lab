using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "PlayerLibraryData", menuName = "Libraries/Create Player Library")]
public class PlayerLibrary : ScriptableObject
{
    public SpriteLibraryAsset[] Library;
}
