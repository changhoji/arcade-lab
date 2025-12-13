using System;
using System.Collections.Generic;
using System.Linq;
using ArcadeLab.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class RoomManager : MonoBehaviour
{
    public event Action<RoomData[]> OnRoomListResponse;
    public event Action<CreateRoomResponse> OnCreateRoomResposne;
    public event Action<JoinRoomResponse> OnJoinRoomResponse;
    public event Action OnLeaveRoomResponse;
    public event Action<RoomPlayerData> OnRoomJoined;
    public event Action<string> OnRoomLeft; 
    public event Action<string, bool> OnReadyChanged;

    public bool IsInRoom => m_CurrentRoom != null;

    [Inject] AuthManager m_AuthManager;
    [Inject] LobbyNetworkService m_LobbyService;

    RoomData m_CurrentRoom;
    Dictionary<string, RoomPlayerData> m_Players = new();
    Dictionary<string, GameConfig> m_GameConfigs = new();
    

    void Start()
    {
        m_LobbyService.OnRoomListResponse += HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne += HandleCreateRoomResponse;
        m_LobbyService.OnJoinRoomResponse += HandleJoinRoomResponse;
        m_LobbyService.OnLeaveRoomResponse += HandleLeaveRoomResponse;
        m_LobbyService.OnStartRoomResponse += HandleStartRoomResponse;
        m_LobbyService.OnRoomJoined += HandleRoomJoined;
        m_LobbyService.OnRoomLeft += HandleRoomLeft;
        m_LobbyService.OnRoomStarted += HandleRoomStarted;
        m_LobbyService.OnReadyChanged += HandleReadyChanged;

        GameConfig[] configs = Resources.LoadAll<GameConfig>("Configs");
        foreach (var config in configs)
        {
            m_GameConfigs[config.gameId] = config;
        }    
    }

    void OnDestroy()
    {
        m_LobbyService.OnRoomListResponse -= HandleRoomListResponse;
        m_LobbyService.OnCreateRoomResposne -= HandleCreateRoomResponse;
        m_LobbyService.OnJoinRoomResponse -= HandleJoinRoomResponse;
        m_LobbyService.OnLeaveRoomResponse -= HandleLeaveRoomResponse;
        m_LobbyService.OnStartRoomResponse -= HandleStartRoomResponse;
        m_LobbyService.OnRoomJoined -= HandleRoomJoined;
        m_LobbyService.OnRoomLeft -= HandleRoomLeft;
        m_LobbyService.OnRoomStarted -= HandleRoomStarted;
        m_LobbyService.OnReadyChanged -= HandleReadyChanged;
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

    public void StartRoom()
    {
        if (m_CurrentRoom == null)
        {
            Debug.LogWarning("player is not in a room");
            return;
        }

        m_LobbyService.RequestStartRoom();
    }

    public void ToggleReady()
    {
        m_Players[m_AuthManager.UserId].isReady = !m_Players[m_AuthManager.UserId].isReady;
        m_LobbyService.SendReady(m_Players[m_AuthManager.UserId].isReady);
        OnReadyChanged?.Invoke(m_AuthManager.UserId, m_Players[m_AuthManager.UserId].isReady);
    }

    void HandleRoomListResponse(RoomData[] rooms)
    {
        OnRoomListResponse?.Invoke(rooms);
    }

    void HandleCreateRoomResponse(CreateRoomResponse response)
    {
        m_CurrentRoom = response.room;
        m_Players.Add(m_AuthManager.UserId, response.player);
        GetRoomList(response.room.gameId);
        OnCreateRoomResposne?.Invoke(response);
    }

    void HandleJoinRoomResponse(JoinRoomResponse response)
    {
        m_CurrentRoom = response.room;
        foreach (var player in response.players)
        {
            m_Players.Add(player.userId, player);
        }
        OnJoinRoomResponse?.Invoke(response);
    }

    void HandleLeaveRoomResponse()
    {
        m_CurrentRoom = null;
        OnLeaveRoomResponse?.Invoke();
        m_LobbyService.RequestRoomList("color-lab");
        m_Players.Clear();
    }

    void HandleStartRoomResponse()
    {
        if (m_GameConfigs.TryGetValue(m_CurrentRoom.gameId, out GameConfig config))
        {
            SceneManager.LoadScene(config.sceneName);
        }
        else
        {
            Debug.LogError($"Gameconfig not found for gmaeId: {m_CurrentRoom.gameId}");
        }
    }

    void HandleRoomJoined(RoomPlayerData player)
    {
        OnRoomJoined?.Invoke(player);
        m_Players.Add(player.userId, player);
    }

    void HandleRoomLeft(string userId)
    {
        OnRoomLeft?.Invoke(userId);
        m_Players.Remove(userId);
    }

    void HandleRoomStarted()
    {
        if (m_GameConfigs.TryGetValue(m_CurrentRoom.gameId, out GameConfig config))
        {
            SceneManager.LoadScene(config.sceneName);
        }
        else
        {
            Debug.LogError($"Gameconfig not found for gmaeId: {m_CurrentRoom.gameId}");
        }
    }

    void HandleReadyChanged(string userId, bool isReady)
    {
        if (!m_Players.ContainsKey(userId))
        {
            Debug.LogWarning("No player exists to modify ready");
            return;
        }

        m_Players[userId].isReady = isReady;
        OnReadyChanged?.Invoke(userId, isReady);
    }
}
