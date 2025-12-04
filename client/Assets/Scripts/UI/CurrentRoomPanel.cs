using ArcadeLab.Data;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class CurrentRoomPanel : UIPanelBase
{
    [SerializeField] Button m_StartButton;
    [SerializeField] Button m_ReadyButton;
    [SerializeField] Button m_LeaveButton;

    [Inject] AuthManager m_AuthManager;
    [Inject] RoomManager m_RoomManager;

    RoomPlayerItem[] m_Players;

    void Start()
    {
        m_RoomManager.OnCreateRoomResposne += HandleCreateRoomResponse;
        m_RoomManager.OnJoinRoomResponse += HandleJoinRoomResponse;
        m_RoomManager.OnLeaveRoomResponse += Hide;
        m_RoomManager.OnRoomJoined += HandleRoomJoined;
        m_RoomManager.OnRoomLeft += HandleRoomLeft;

        m_LeaveButton.onClick.AddListener(() => m_RoomManager.LeaveRoom());

        m_Players = GetComponentsInChildren<RoomPlayerItem>();

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        m_RoomManager.OnCreateRoomResposne -= HandleCreateRoomResponse;
        m_RoomManager.OnJoinRoomResponse -= HandleJoinRoomResponse;
        m_RoomManager.OnLeaveRoomResponse += Hide;
        m_RoomManager.OnRoomJoined -= HandleRoomJoined;
        m_RoomManager.OnRoomLeft -= HandleRoomLeft;
    }
   
    void HandleCreateRoomResponse(RoomData room)
    {
        m_Players[0].SetData(new RoomPlayerData
        {
            userId = m_AuthManager.UserId,
            nickname = m_AuthManager.Player.nickname,
            skinIndex = m_AuthManager.Player.skinIndex,
            isHost = true,
            isReady = false
        });
    }

    void HandleJoinRoomResponse(JoinRoomResponse response)
    {
        var players = response.players;
        Debug.Log(players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            var player = players[i];
            m_Players[i].SetData(player);
        }
    }

    void HandleRoomJoined(RoomPlayerData player)
    {
        Debug.Log("handle room joined in current room panel");
        m_Players[1].SetData(player);
    }

    void HandleRoomLeft(string userId)
    {
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].PlayerData.userId == userId)
            {
                m_Players[i].Clear();
            }
        }
    }
}
