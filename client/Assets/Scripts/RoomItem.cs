using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TMP_Text m_RoomNameText;
    [SerializeField] TMP_Text m_PlayerCountText;
    [SerializeField] Button m_JoinButton;

    RoomData m_RoomData;

    public void Initialize(RoomData roomData)
    {
        m_RoomData = roomData;

        m_RoomNameText.text = roomData.roomName;
        m_PlayerCountText.text = $"{roomData.currentPlayers} / {roomData.maxPlayers}";
    }
}
