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

    public bool IsInRoom => m_CurrentRoom != null;

    [Inject] LobbyNetworkService m_LobbyService;

    RoomData m_CurrentRoom;

    void Start()
    {
        m_LobbyService.OnRoomListResponse += HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne += HandleCreateRoomResponse;
        m_LobbyService.OnJoinRoomResponse += HandleJoinRoomResponse;
    }

    void OnDestroy()
    {
        m_LobbyService.OnRoomListResponse -= HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne -= HandleCreateRoomResponse;
        m_LobbyService.OnJoinRoomResponse -= HandleJoinRoomResponse;
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
        if (room == null)
        {
            Debug.LogError("failed create room");
            return;
        }

        m_CurrentRoom = room;
        OnCreateRoomResposne?.Invoke(room);
    }

    void HandleJoinRoomResponse(JoinRoomResponse response)
    {
        m_CurrentRoom = response.room;
        OnJoinRoomResponse?.Invoke(response);
    }
}
