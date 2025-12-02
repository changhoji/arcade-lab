using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using VContainer;

public class RoomPlayerItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_NicknameText;
    [SerializeField] TextMeshProUGUI m_IsReadyText;

    RoomPlayerData m_Player;
    
    public void SetData(RoomPlayerData player)
    {
        m_Player = player;

        m_NicknameText.text = player.nickname;
        m_IsReadyText.text = player.isReady ? "O" : "X";
    }
}
