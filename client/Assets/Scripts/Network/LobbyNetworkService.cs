using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcadeLab.Data;
using NUnit.Framework;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using VContainer;

public class LobbyNetworkService : INetworkService
{
    public event Action<LobbyData[]> OnLobbyListResponse;
    public event Action<string> OnCreateLobbyResponse;
    public event Action<string> OnJoinLobbyResponse;
    public event Action<RoomData[]> OnRoomListResponse;
    public event Action<RoomData> OnCreateRoomResposne;
    public event Action<JoinRoomResponse> OnJoinRoomResponse;

    // lobby sync events
    public event Action<LobbyPlayerData[]> OnLobbyInitResponse;
    public event Action<string, Position> OnPositionChanged;
    public event Action<string, bool> OnIsMovingChanged;
    public event Action<LobbyPlayerData> OnPlayerJoined;
    public event Action<string> OnPlayerLeft;
    public event Action<string, int> OnSkinChanged;
    public event Action<string, string> OnNicknameChanged;
    
    // room sync events
    public event Action<RoomPlayerData> OnRoomJoined;

    [Inject] AuthManager m_AuthManager;
    SocketIOUnity m_LobbySocket;

    public void Initialize()
    {
    }

    public void InitializeSocket()
    {
        m_LobbySocket = new SocketIOUnity("http://localhost:3000/lobby", new SocketIOOptions
        {
            Auth = new Dictionary<string, string>
            {
                { "userId", m_AuthManager.UserId }
            }
        });
        m_LobbySocket.JsonSerializer = new NewtonsoftJsonSerializer();
        RegisterEventListeners();
    }

    public void Dispose()
    {
        Disconnect();
    }

    public void RegisterEventListeners()
    {
        m_LobbySocket.OnUnityThread("player:positionChanged", response =>
        {
            var userId = response.GetValue<string>(0);
            var position = response.GetValue<Position>(1);

            OnPositionChanged?.Invoke(userId, position);
        });

        m_LobbySocket.OnUnityThread("player:isMovingChanged", response =>
        {
            var userId = response.GetValue<string>(0);
            var isMoving = response.GetValue<bool>(1);

            OnIsMovingChanged?.Invoke(userId, isMoving);
        });

        m_LobbySocket.OnUnityThread("player:joined", response =>
        {
            var player = response.GetValue<LobbyPlayerData>(0);

            OnPlayerJoined?.Invoke(player);
        });

        m_LobbySocket.OnUnityThread("player:left", resposne =>
        {
            var userId = resposne.GetValue<string>(0);
            
            OnPlayerLeft?.Invoke(userId);
        });

        m_LobbySocket.OnUnityThread("player:skinChanged", response =>
        {
            var userId = response.GetValue<string>(0);
            var skinIndex = response.GetValue<int>(1);

            OnSkinChanged?.Invoke(userId, skinIndex);
        });

        m_LobbySocket.OnUnityThread("player:nicknameChanged", response =>
        {
            var userId = response.GetValue<string>(0);
            var nickname = response.GetValue<string>(1);

            OnNicknameChanged?.Invoke(userId, nickname);
        });

        m_LobbySocket.OnUnityThread("room:joined", response =>
        {
            var player = response.GetValue<RoomPlayerData>();

            OnRoomJoined?.Invoke(player);
        });
    }

    public async Task ConnectAsync()
    {
        await m_LobbySocket.ConnectAsync();
    }

    public void Disconnect()
    {
        if (m_LobbySocket != null)
        {
            m_LobbySocket.Disconnect();    
        }
    }

    public void RequestLobbyList()
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("lobby:list", (response) =>
        {
            var result = response.GetValue<NetworkResult<LobbyData[]>>();
            if (result.success)
            {
                context.Post(_ => OnLobbyListResponse?.Invoke(result.data), null);    
            }
            else
            {
                Debug.LogError($"Get lobby list failed: {result.error}");
            }
        });
    }

    public void RequestCreateLobby(string name)
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("lobby:create", (response) =>
        {
            var result = response.GetValue<NetworkResult<string>>();
            if (result.success)
            {
                context.Post(_ => OnCreateLobbyResponse?.Invoke(result.data), null);
            }
            else
            {
                Debug.LogError($"Lobby creation failed: {result.error}");
            }
        }, name);
    }

    public void RequestJoinLobby(string lobbyId)
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("lobby:join", (response) =>
        {
            var result = response.GetValue<NetworkResult<string>>();
            if (result.success)
            {
                context.Post(_ => OnJoinLobbyResponse?.Invoke(result.data), null);    
            }
            else
            {
                Debug.LogError($"Lobby join failed: {result.error}");
            }
        }, lobbyId);
    }

    public void RequestLobbyInit()
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("lobby:init", (response) =>
        {
            var result = response.GetValue<NetworkResult<LobbyPlayerData[]>>();
            if (result.success)
            {
                context.Post(_ => OnLobbyInitResponse?.Invoke(result.data), null);
            }
            else
            {
                Debug.LogError($"Lobby init failed: {result.error}");
            }
        });
    }

    public void RequestRoomList(string gameId)
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("room:list", (response) =>
        {
            var result = response.GetValue<NetworkResult<RoomData[]>>();
            if (result.success)
            {
                context.Post(_ => OnRoomListResponse?.Invoke(result.data), null);
            }
            else
            {
                Debug.LogError($"Get room list failed: {result.error}");
            }
        }, gameId);
    }

    public void RequestCreateRoom(string gameId, string name)
    {
        var request = new CreateRoomRequest
        {
            gameId = gameId, 
            name = name
        };
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("room:create", (response) =>
        {
            var result = response.GetValue<NetworkResult<RoomData>>();
            if (result.success)
            {
                context.Post(_ => OnCreateRoomResposne?.Invoke(result.data), null);    
            }
            else
            {
                Debug.LogError($"Room creation failed: {result.error}");
            }
        }, request);
    }

    public void RequestJoinRoom(string roomId)
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("room:join", (response) =>
        {
            var result = response.GetValue<NetworkResult<JoinRoomResponse>>();
            if (result.success)
            {
                context.Post(_ => OnJoinRoomResponse?.Invoke(result.data), null);
            }
            else
            {
                Debug.LogError($"Room join failed: {result.error}");
            }
        }, roomId);
    }

    public void EmitPlayerMoved(Position position)
    {
        m_LobbySocket.Emit("player:changePosition", position);
    }

    public void EmitPlayerMoving(bool value)
    {
        m_LobbySocket.Emit("player:changeIsMoving", value);
    }

    public void EmitPlayerSkinIndex(int skinIndex)
    {
        m_LobbySocket.Emit("player:changeSkin", skinIndex);
    }

    public void EmitPlayerNickname(string nickname)
    {
        m_LobbySocket.Emit("player:changeNickname", nickname);
    }
}
