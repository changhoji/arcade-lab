using System;
using System.Collections.Generic;
using System.Linq;
using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class RoomManager : MonoBehaviour
{
    public event Action<RoomData[]> OnRoomListResponse;

    [Inject] LobbyNetworkService m_LobbyService;

    void Start()
    {
        m_LobbyService.OnRoomListResponse += HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne += HandleCreateRoomResponse;
    }

    void OnDestroy()
    {
        m_LobbyService.OnRoomListResponse -= HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne -= HandleCreateRoomResponse;
    }

    public void GetRoomList(string gameId)
    {
        m_LobbyService.RequestRoomList(gameId);
    }

    public void CreateRoom(string gameId, string name)
    {
        m_LobbyService.RequestCreateRoom(gameId, name);
    }

    public void JoinRoom(string roomId)
    {
        m_LobbyService.RequestJoinRoom(roomId);
    }

    void HandleRoomListResponse(RoomData[] rooms)
    {
        OnRoomListResponse?.Invoke(rooms);
    }

    void HandleCreateRoomResponse(string roomId)
    {
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("failed create room");
            return;
        }

        // m_Panel.UpdateRooms
    }

    // public RoomData[] GetRoomDatas(string gameId)
    // {
    //     return m_Rooms.Values.Where(room => room.gameId == gameId).ToArray();
    // }

    // void HandleRoomCreated(RoomData roomData)
    // {
    //     Debug.Log("add room");
    //     m_Rooms.Add(roomData.roomId, roomData);
    // }

    // void HandleRoomDeleted(string roomId)
    // {
    //     m_Rooms.Remove(roomId);
    // }

    // void OnDestroy()
    // {
    //     m_LobbyService.OnRoomCreated -= HandleRoomCreated;
    //     m_LobbyService.OnRoomDeleted -= HandleRoomDeleted;
    // }
}
