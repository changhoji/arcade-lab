using System;
using System.Collections.Generic;
using System.Linq;
using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class RoomManager : MonoBehaviour
{
    public event Action<RoomData[]> OnRoomListResponse;
    public event Action<CreateRoomResponse> OnCreateRoomResposne;
    public event Action<JoinRoomResponse> OnJoinRoomResponse;
    public event Action OnLeaveRoomResponse;
    public event Action<RoomPlayerData> OnRoomJoined;
    public event Action<string> OnRoomLeft; 

    public bool IsInRoom => m_CurrentRoom != null;

    [Inject] LobbyNetworkService m_LobbyService;

    RoomData m_CurrentRoom;
    RoomPlayerData m_PlayerData;

    void Start()
    {
        m_LobbyService.OnRoomListResponse += HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne += HandleCreateRoomResponse;
        m_LobbyService.OnJoinRoomResponse += HandleJoinRoomResponse;
        m_LobbyService.OnLeaveRoomResponse += HandleLeaveRoomResponse;
        m_LobbyService.OnRoomJoined += HandleRoomJoined;
        m_LobbyService.OnRoomLeft += HandleRoomLeft;

    }

    void OnDestroy()
    {
        m_LobbyService.OnRoomListResponse -= HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne -= HandleCreateRoomResponse;
        m_LobbyService.OnJoinRoomResponse -= HandleJoinRoomResponse;
        m_LobbyService.OnLeaveRoomResponse -= HandleLeaveRoomResponse;
        m_LobbyService.OnRoomJoined -= HandleRoomJoined;
        m_LobbyService.OnRoomLeft -= HandleRoomLeft;
    }

    public void GetRoomList(string gameId)
    {
        m_LobbyService.RequestRoomList(gameId);
    }

    public void CreateRoom(string gameId, string name)
    {
        if (m_CurrentRoom != null)
        {
            Debug.LogWarning("player already in a room");
            return;
        }

        m_LobbyService.RequestCreateRoom(gameId, name);
    }

    public void JoinRoom(string roomId)
    {
        if (m_CurrentRoom != null)
        {
            Debug.LogWarning("player already in a room");
            return;
        }

        m_LobbyService.RequestJoinRoom(roomId);
    }

    public void LeaveRoom()
    {
        if (m_CurrentRoom == null)
        {
            Debug.LogWarning("player is not in a room");
            return;
        }

        m_LobbyService.RequestLeaveRoom();
    }

    public void ChangeIsReady(bool isReady)
    {
        m_PlayerData.isReady = isReady;
        // m_LobbyService.Emit
    }

    void HandleRoomListResponse(RoomData[] rooms)
    {
        OnRoomListResponse?.Invoke(rooms);
    }

    void HandleCreateRoomResponse(CreateRoomResponse response)
    {
        m_CurrentRoom = response.room;
        m_PlayerData = response.player;
        GetRoomList(response.room.gameId);
        OnCreateRoomResposne?.Invoke(response);
    }

    void HandleJoinRoomResponse(JoinRoomResponse response)
    {
        m_CurrentRoom = response.room;
        OnJoinRoomResponse?.Invoke(response);
    }

    void HandleLeaveRoomResponse()
    {
        m_CurrentRoom = null;
        OnLeaveRoomResponse?.Invoke();
        m_LobbyService.RequestRoomList("color-lab");
    }

    void HandleRoomJoined(RoomPlayerData player)
    {
        OnRoomJoined?.Invoke(player);
    }

    void HandleRoomLeft(string userId)
    {
        OnRoomLeft?.Invoke(userId);
    }
}
