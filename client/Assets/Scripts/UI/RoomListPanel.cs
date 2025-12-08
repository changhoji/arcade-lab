using System;
using System.Collections.Generic;
using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class RoomListPanel : UIPanelBase
{
    [Inject] RoomManager m_RoomManager;

    ScrollView m_ScrollView;
    TextField m_NameInput;
    Button m_CreateButton;
    Button m_RefreshButton;

    List<VisualElement> m_RoomRows = new();
    GameConfig m_GameConfig;

    protected override void Awake()
    {
        base.Awake();

        m_ScrollView = m_Root.Q<ScrollView>("room-list-scroll");
        m_NameInput = m_Root.Q<TextField>("room-name-input");
        m_CreateButton = m_Root.Q<Button>("create-button");
        m_RefreshButton = m_Root.Q<Button>("refresh-button");
    }

    protected override void Update()
    {
        base.Update();
    }

    void Start()
    {
        m_RoomManager.OnRoomListResponse += UpdateRooms;

        m_CreateButton.clicked += () => 
        {
            if (!string.IsNullOrEmpty(m_NameInput.text) && m_GameConfig != null)
            {
                m_RoomManager.CreateRoom(m_GameConfig.gameId, m_NameInput.text);
                m_NameInput.value = ""; // 입력 필드 초기화
            }
        };

        m_RefreshButton.clicked += () => 
        {
            if (m_GameConfig != null)
            {
                m_RoomManager.GetRoomList(m_GameConfig.gameId);
            }
        };

        Hide();
    }

    void OnDestroy()
    {
        m_RoomManager.OnRoomListResponse -= UpdateRooms;
    }

    public void UpdateRooms(RoomData[] rooms)
    {
        m_ScrollView.Clear();
        m_RoomRows.Clear();

        foreach (var room in rooms)
        {
            var row = CreateRoomRow(room);
            m_ScrollView.Add(row);
            m_RoomRows.Add(row);
        }
    }

    VisualElement CreateRoomRow(RoomData room)
    {
        var row = new VisualElement();
        row.AddToClassList("room-row");

        // 방 이름
        var nameLabel = new Label(room.name);
        nameLabel.AddToClassList("column");
        nameLabel.AddToClassList("column-name");

        // 플레이어 수
        var playersLabel = new Label($"{room.currentPlayers} / {room.maxPlayers}");
        playersLabel.AddToClassList("column");
        playersLabel.AddToClassList("column-players");

        // Join 버튼
        var joinButton = new Button(() => m_RoomManager.JoinRoom(room.roomId));
        joinButton.text = "Join";
        joinButton.AddToClassList("join-button");
        
        // 방이 꽉 찼으면 버튼 비활성화
        if (room.currentPlayers >= room.maxPlayers)
        {
            joinButton.SetEnabled(false);
        }

        var actionContainer = new VisualElement();
        actionContainer.AddToClassList("column");
        actionContainer.AddToClassList("column-action");
        actionContainer.Add(joinButton);

        row.Add(nameLabel);
        row.Add(playersLabel);
        row.Add(actionContainer);

        return row;
    }

    public void SetGameConfig(GameConfig config)
    {
        m_GameConfig = config;
    }

    public override void Show()
    {
        base.Show();
        
        if (m_GameConfig != null)
        {
            m_RoomManager.GetRoomList(m_GameConfig.gameId);
        }
    }

    public override void Hide()
    {
        base.Hide();

        Debug.Log("roomlistpanel hide");
    }
}
