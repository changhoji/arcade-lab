using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TMP_Text m_RoomNameText;
    [SerializeField] TMP_Text m_PlayerCountText;
    [SerializeField] Button m_JoinButton;

    RoomData m_RoomData;
    GameConfig m_GameConfig;

    public void Initialize(RoomData roomData, GameConfig gameConfig)
    {
        m_RoomData = roomData;
        m_GameConfig = gameConfig;

        m_RoomNameText.text = roomData.roomName;
        m_PlayerCountText.text = $"{roomData.currentPlayers} / {roomData.maxPlayers}";

        m_JoinButton.onClick.AddListener(OnClickJoin);
    }

    void OnClickJoin()
    {
        SceneManager.LoadScene(m_GameConfig.sceneName);
    }
}
