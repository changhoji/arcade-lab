using System.Collections.Generic;
using System.Linq;
using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class RoomManager : MonoBehaviour
{
    LobbyNetworkService m_LobbyService;
    LobbyUIManager m_LobbyUIManager;
    Dictionary<string, RoomData> m_Rooms;

    [Inject]
    public void Construct(LobbyUIManager lobbyUIManager, LobbyNetworkService lobbyService)
    {
        m_Rooms = new();
        m_LobbyService = lobbyService;
        m_LobbyUIManager = lobbyUIManager;

        m_LobbyService.OnRoomCreated += HandleRoomCreated;
        m_LobbyService.OnRoomDeleted += HandleRoomDeleted;
    }

    public void CreateRoom(string gameId, string roomName, int maxPlayers)
    {
        m_LobbyService.EmitCreateRoom(gameId, roomName, maxPlayers);
    }

    public RoomData[] GetRoomDatas(string gameId)
    {
        return m_Rooms.Values.Where(room => room.gameId == gameId).ToArray();
    }

    void HandleRoomCreated(RoomData roomData)
    {
        m_Rooms.Add(roomData.roomId, roomData);
        m_LobbyUIManager.ShowRoomListPanel(roomData.gameId);
    }

    void HandleRoomDeleted(string roomId)
    {
        m_Rooms.Remove(roomId);
    }

    void OnDestroy()
    {
        m_LobbyService.OnRoomCreated -= HandleRoomCreated;
        m_LobbyService.OnRoomDeleted -= HandleRoomDeleted;
    }
}
