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
        m_Players = GetComponentsInChildren<RoomPlayerItem>();

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        m_RoomManager.OnCreateRoomResposne -= HandleCreateRoomResponse;
    }
   
    void HandleCreateRoomResponse(RoomData room)
    {
        if (room == null)
        {
            return;
        }

        m_Players[0].SetData(new RoomPlayerData
        {
            userId = m_AuthManager.UserId,
            nickname = m_AuthManager.Player.nickname,
            skinIndex = m_AuthManager.Player.skinIndex,
            isHost = true,
            isReady = false
        });
    }
}
