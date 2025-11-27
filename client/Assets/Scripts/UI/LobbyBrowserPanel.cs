using System;
using System.Collections.Generic;
using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LobbyBrowserPanel : UIPanelBase
{
    public event Action OnClickRefresh;
    public event Action<string> OnClickCreate;
    public event Action<string> OnClickJoin;

    [SerializeField] Transform m_ScrollContent;
    [SerializeField] GameObject m_LobbyItemPrefab;
    [SerializeField] TMP_InputField m_NameInput;
    [SerializeField] Button m_CreateButton;
    [SerializeField] Button m_RefreshButton;

    List<LobbyItem> m_LobbyItems = new();

    void Start()
    {
        m_CreateButton.onClick.AddListener(() => OnClickCreate?.Invoke(m_NameInput.text));
        m_RefreshButton.onClick.AddListener(() => OnClickRefresh?.Invoke());
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
            lobbyItem.OnClickJoin += (lobbyId) => OnClickJoin?.Invoke(lobbyId);
            lobbyItem.transform.Translate(new Vector3(0, -50*i, 0));
            m_LobbyItems.Add(lobbyItem);
        }
    }
}
