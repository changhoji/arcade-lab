using System;
using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    public event Action<string> OnClickJoin;

    [SerializeField] TMP_Text m_NameText;
    [SerializeField] TMP_Text m_PlayersText;
    [SerializeField] Button m_JoinButton;

    LobbyData m_LobbyData;

    public void Init(LobbyData data)
    {
        m_LobbyData = data;
        m_NameText.text = data.name;
        m_PlayersText.text = $"joined: {data.currentPlayers}";
        m_JoinButton.onClick.AddListener(() => OnClickJoin?.Invoke(m_LobbyData.lobbyId));

    }
}
