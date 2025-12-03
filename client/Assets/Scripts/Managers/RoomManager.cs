using System;
using System.Collections.Generic;
using System.Linq;
using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class RoomManager : MonoBehaviour
{
    public event Action<RoomData[]> OnRoomListResponse;
    public event Action<RoomData> OnCreateRoomResposne;
    public event Action<JoinRoomResponse> OnJoinRoomResponse;
    public event Action<RoomPlayerData> OnRoomJoined;

    public bool IsInRoom => m_CurrentRoom != null;

    [Inject] LobbyNetworkService m_LobbyService;

    RoomData m_CurrentRoom;

    void Start()
    {
        m_LobbyService.OnRoomListResponse += HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne += HandleCreateRoomResponse;
        m_LobbyService.OnJoinRoomResponse += HandleJoinRoomResponse;
        m_LobbyService.OnRoomJoined += HandleRoomJoined;
    }

    void OnDestroy()
    {
        m_LobbyService.OnRoomListResponse -= HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne -= HandleCreateRoomResponse;
        m_LobbyService.OnJoinRoomResponse -= HandleJoinRoomResponse;
        m_LobbyService.OnRoomJoined -= HandleRoomJoined;
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

    void HandleRoomListResponse(RoomData[] rooms)
    {
        OnRoomListResponse?.Invoke(rooms);
    }

    void HandleCreateRoomResponse(RoomData room)
    {
        m_CurrentRoom = room;
        GetRoomList(room.gameId);
        OnCreateRoomResposne?.Invoke(room);
    }

    void HandleJoinRoomResponse(JoinRoomResponse response)
    {
        m_CurrentRoom = response.room;
        OnJoinRoomResponse?.Invoke(response);
    }

    void HandleRoomJoined(RoomPlayerData player)
    {
        OnRoomJoined?.Invoke(player);
    }
}
