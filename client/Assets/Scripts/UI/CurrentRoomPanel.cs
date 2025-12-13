using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class CurrentRoomPanel : UIPanelBase
{
    [Inject] AuthManager m_AuthManager;
    [Inject] LobbyManager m_LobbyManager;
    [Inject] RoomManager m_RoomManager;

    Button m_StartButton;
    Button m_ReadyButton;
    Button m_LeaveButton;

    RoomPlayerData[] m_PlayersData = new RoomPlayerData[2];

    VisualElement[] m_PlayerSlots;
    Label[] m_PlayerNicknames;
    Label[] m_PlayerHostBadges;
    Label[] m_PlayerReadyBadges;

    protected override void Awake()
    {
        base.Awake();

        m_StartButton = m_Root.Q<Button>("start-button");
        m_ReadyButton = m_Root.Q<Button>("ready-button");
        m_LeaveButton = m_Root.Q<Button>("leave-button");

        m_PlayerSlots = new VisualElement[2];
        m_PlayerNicknames = new Label[2];
        m_PlayerHostBadges = new Label[2];
        m_PlayerReadyBadges = new Label[2];

        for (int i = 0; i < 2; i++)
        {
            m_PlayerSlots[i] = m_Root.Q<VisualElement>($"player-slot-{i}");
            m_PlayerNicknames[i] = m_Root.Q<Label>($"player-nickname-{i}");
            m_PlayerHostBadges[i] = m_Root.Q<Label>($"player-host-{i}");
            m_PlayerReadyBadges[i] = m_Root.Q<Label>($"player-ready-{i}");
            
            m_PlayerSlots[i].AddToClassList("empty");
        }
    }

    void Start()
    {
        m_LobbyManager.OnNicknameChanged += UpdateNickname;

        m_RoomManager.OnCreateRoomResposne += HandleCreateRoomResponse;
        m_RoomManager.OnJoinRoomResponse += HandleJoinRoomResponse;
        m_RoomManager.OnLeaveRoomResponse += Hide;
        m_RoomManager.OnRoomJoined += HandleRoomJoined;
        m_RoomManager.OnRoomLeft += HandleRoomLeft;
        m_RoomManager.OnReadyChanged += UpdateReady;

        m_LeaveButton.clicked += m_RoomManager.LeaveRoom;
        m_ReadyButton.clicked += m_RoomManager.ToggleReady;;
        m_StartButton.clicked += m_RoomManager.StartRoom;

        Hide();
    }

    void OnDestroy()
    {
        m_LobbyManager.OnNicknameChanged -= UpdateNickname;

        m_RoomManager.OnCreateRoomResposne -= HandleCreateRoomResponse;
        m_RoomManager.OnJoinRoomResponse -= HandleJoinRoomResponse;
        m_RoomManager.OnLeaveRoomResponse += Hide;
        m_RoomManager.OnRoomJoined -= HandleRoomJoined;
        m_RoomManager.OnRoomLeft -= HandleRoomLeft;
        m_RoomManager.OnReadyChanged -= UpdateReady;
    }
   
    void HandleCreateRoomResponse(CreateRoomResponse response)
    {
        SetPlayerData(0, response.player);

        m_StartButton.style.display = DisplayStyle.Flex;
        m_ReadyButton.style.display = DisplayStyle.None;
    }

    void HandleJoinRoomResponse(JoinRoomResponse response)
    {
        var players = response.players;
        
        for (int i = 0; i < players.Length; i++)
        {
            SetPlayerData(i, players[i]);
        }

        m_StartButton.style.display = DisplayStyle.None;
        m_ReadyButton.style.display = DisplayStyle.Flex;
    }

    void HandleRoomJoined(RoomPlayerData player)
    {
        SetPlayerData(1, player);
    }

    void HandleRoomLeft(string userId)
    {
        for (int i = 0; i < m_PlayersData.Length; i++)
        {
            if (m_PlayersData[i] != null && m_PlayersData[i].userId == userId)
            {
                ClearPlayerSlot(i);
            }
        }
    }

    void SetPlayerData(int slotIndex, RoomPlayerData player)
    {
        if (slotIndex < 0 || slotIndex >= 2) return;

        m_PlayersData[slotIndex] = player;

        // 슬롯 스타일 업데이트
        m_PlayerSlots[slotIndex].RemoveFromClassList("empty");
        m_PlayerSlots[slotIndex].AddToClassList("occupied");

        // 닉네임 표시
        m_PlayerNicknames[slotIndex].text = player.nickname;

        // Host 뱃지 표시
        m_PlayerHostBadges[slotIndex].style.display = 
            player.isHost ? DisplayStyle.Flex : DisplayStyle.None;

        m_PlayerReadyBadges[slotIndex].style.display =
            player.isHost ? DisplayStyle.None : DisplayStyle.Flex;
        // Ready 뱃지 업데이트
        UpdateReadyBadge(slotIndex, player.isReady);
    }

    void ClearPlayerSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= 2) return;

        m_PlayersData[slotIndex] = null;

        m_PlayerSlots[slotIndex].RemoveFromClassList("occupied");
        m_PlayerSlots[slotIndex].AddToClassList("empty");

        m_PlayerNicknames[slotIndex].text = "Empty";
        m_PlayerHostBadges[slotIndex].style.display = DisplayStyle.None;
        m_PlayerReadyBadges[slotIndex].text = "";
        m_PlayerReadyBadges[slotIndex].RemoveFromClassList("ready");
        m_PlayerReadyBadges[slotIndex].RemoveFromClassList("not-ready");
    }

    void UpdateReadyBadge(int slotIndex, bool isReady)
    {
        var badge = m_PlayerReadyBadges[slotIndex];
        
        badge.RemoveFromClassList("ready");
        badge.RemoveFromClassList("not-ready");

        if (isReady)
        {
            badge.text = "READY";
            badge.AddToClassList("ready");
        }
        else
        {
            badge.text = "NOT READY";
            badge.AddToClassList("not-ready");
        }
    }

    void UpdateNickname(string userId, string nickname)
    {
        if (m_PlayersData[0] != null && m_PlayersData[0].userId == userId)
        {
            m_PlayerNicknames[0].text = nickname;
        }
        if (m_PlayersData[1] != null && m_PlayersData[1].userId == userId)
        {
            m_PlayerNicknames[1].text = nickname;
        }
    }

    void UpdateReady(string userId, bool isReady)
    {
        if (m_PlayersData[0] != null && m_PlayersData[0].userId == userId)
        {
            UpdateReadyBadge(0, isReady);
        }
        if (m_PlayersData[1] != null && m_PlayersData[1].userId == userId)
        {
            UpdateReadyBadge(1, isReady);
        }
    }
}
