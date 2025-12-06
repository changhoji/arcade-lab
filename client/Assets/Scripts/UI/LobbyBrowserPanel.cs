using System;
using System.Collections.Generic;
using ArcadeLab.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class LobbyBrowserPanel : UIPanelBase
{
    [Inject] LobbyBrowserManager m_Manager;

    ScrollView m_ScrollView;
    TextField m_NameInput;
    Button m_CreateButton;
    Button m_RefreshButton;

    List<VisualElement> m_LobbyRows = new();

    protected override void Awake()
    {
        base.Awake();

        m_ScrollView = m_Root.Q<ScrollView>("lobby-list-scroll");
        m_NameInput = m_Root.Q<TextField>("lobby-name-input");
        m_CreateButton = m_Root.Q<Button>("create-lobby-button");
        m_RefreshButton = m_Root.Q<Button>("refresh-lobby-button");
    }

    void Start()
    {
        m_Manager.OnLobbyListResponse += UpdateLobbies;

        m_CreateButton.clicked += () => m_Manager.CreateLobby(m_NameInput.text);
    }

    void OnDestroy()
    {
        m_Manager.OnLobbyListResponse -= UpdateLobbies;

        m_CreateButton.clicked -= () => m_Manager.CreateLobby(m_NameInput.text);
    }

    public void UpdateLobbies(LobbyData[] lobbies)
    {
        m_ScrollView.Clear();
        m_LobbyRows.Clear();

        foreach (var lobby in lobbies)
        {
            var row = CreateLobbyRow(lobby);
            m_ScrollView.Add(row);
            m_LobbyRows.Add(row);
        }
    }

    VisualElement CreateLobbyRow(LobbyData lobby)
    {
        var row = new VisualElement();
        row.AddToClassList("data-row");

        var nameLabel = new Label(lobby.name);
        nameLabel.AddToClassList("column");
        nameLabel.AddToClassList("column-name");

        var hostLabel = new Label("-");
        hostLabel.AddToClassList("column");
        hostLabel.AddToClassList("column-host");

        var playersLabel = new Label($"{lobby.currentPlayers}");
        playersLabel.AddToClassList("column");
        playersLabel.AddToClassList("column-players");

        row.Add(nameLabel);
        row.Add(hostLabel);
        row.Add(playersLabel);

        row.RegisterCallback<MouseUpEvent>(evt =>
        {
            m_Manager.JoinLobby(lobby.lobbyId);
        });

        return row;
    }
}
