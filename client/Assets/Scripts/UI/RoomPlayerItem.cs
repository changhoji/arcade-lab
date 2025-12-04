using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using VContainer;

public class RoomPlayerItem : MonoBehaviour
{
    public RoomPlayerData PlayerData;

    [SerializeField] TextMeshProUGUI m_NicknameText;
    [SerializeField] TextMeshProUGUI m_IsReadyText;

    public void SetData(RoomPlayerData player)
    {
        PlayerData = player;

        m_NicknameText.text = player.nickname;
        m_IsReadyText.text = player.isReady ? "O" : "X";
    }

    public void Clear()
    {
        m_NicknameText.text = "";
        m_IsReadyText.text = "";
    }
}
