using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class CurrentRoomPanel : UIPanelBase
{
    [Inject] AuthManager m_AuthManager;
    [Inject] RoomManager m_RoomManager;

    RoomPlayerItem[] m_Players;

    void Start()
    {
        m_RoomManager.OnCreateRoomResposne += HandleCreateRoomResponse;
        m_RoomManager.OnJoinRoomResponse += HandleJoinRoomResponse;
        m_RoomManager.OnRoomJoined += HandleRoomJoined;
        m_Players = GetComponentsInChildren<RoomPlayerItem>();

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        m_RoomManager.OnCreateRoomResposne -= HandleCreateRoomResponse;
        m_RoomManager.OnJoinRoomResponse -= HandleJoinRoomResponse;
        m_RoomManager.OnRoomJoined -= HandleRoomJoined;
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
}
