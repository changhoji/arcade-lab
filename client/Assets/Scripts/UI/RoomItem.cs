using System;
using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public event Action<string> OnClickJoin;

    [SerializeField] TMP_Text m_RoomNameText;
    [SerializeField] TMP_Text m_PlayerCountText;
    [SerializeField] Button m_JoinButton;

    RoomData m_RoomData;
    GameConfig m_GameConfig;

    public void Init(RoomData roomData, GameConfig gameConfig)
    {
        m_RoomData = roomData;
        m_GameConfig = gameConfig;

        m_RoomNameText.text = roomData.name;
        m_PlayerCountText.text = $"{roomData.currentPlayers} / 2";

        m_JoinButton.onClick.AddListener(() => OnClickJoin?.Invoke(m_RoomData.roomId));
    }
}
