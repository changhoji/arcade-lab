using System;
using System.Collections.Generic;
using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LobbyBrowserPanel : UIPanelBase
{
    [SerializeField] Transform m_ScrollContent;
    [SerializeField] GameObject m_LobbyItemPrefab;
    [SerializeField] TMP_InputField m_NameInput;
    [SerializeField] Button m_CreateButton;
    [SerializeField] Button m_RefreshButton;

    [Inject] LobbyBrowserManager m_Manager;

    List<LobbyItem> m_LobbyItems = new();

    void Start()
    {
        m_Manager.OnLobbyListResponse += UpdateLobbies;

        m_CreateButton.onClick.AddListener(() => m_Manager.CreateLobby(m_NameInput.text));
        m_RefreshButton.onClick.AddListener(() => m_Manager.GetLobbyList());
    }

    void OnDestroy()
    {
        m_Manager.OnLobbyListResponse -= UpdateLobbies;
    }

    public void UpdateLobbies(LobbyData[] lobbies)
    {
        foreach (var lobbyItem in m_LobbyItems)
        {
            Destroy(lobbyItem.gameObject);
        }
        m_LobbyItems.Clear();

        for (int i = 0; i < lobbies.Length; i++)
        {
            var lobbyId = lobbies[i].lobbyId;
            var lobbyObject = Instantiate(m_LobbyItemPrefab, m_ScrollContent);
            var lobbyItem = lobbyObject.GetComponent<LobbyItem>();
            lobbyItem.Init(lobbies[i]);
            lobbyItem.OnClickJoin += (lobbyId) => m_Manager.JoinLobby(lobbyId);
            lobbyItem.transform.Translate(new Vector3(0, -50*i, 0));
            m_LobbyItems.Add(lobbyItem);
        }
    }
}
