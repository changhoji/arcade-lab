using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public event Action<LobbyPlayerData[]> OnLobbyInitResponse;
    public event Action<LobbyData[]> OnLobbyListResponse;
    public event Action<string> OnCreateLobbyResponse;
    public event Action<string> OnJoinLobbyResponse;
    public event Action<RoomData[]> OnRoomListResponse;
    public event Action<CreateRoomResponse> OnCreateRoomResposne;
    public event Action<JoinRoomResponse> OnJoinRoomResponse;
    public event Action OnLeaveRoomResponse;
    public event Action OnStartRoomResponse;
    
    public event Action<PlayerPositionPayload> OnPositionChanged;
    public event Action<PlayerMovingPayload> OnMovingChanged;
    public event Action<LobbyPlayerData> OnLobbyJoined;
    public event Action<string> OnLobbyLeft;
    public event Action<PlayerSkinPayload> OnSkinChanged;
    public event Action<PlayerNicknamePayload> OnNicknameChanged;
    public event Action<PlayerReadyPayload> OnReadyChanged;
    public event Action OnRoomStarted;
    public event Action<RoomPlayerData> OnRoomJoined;
    public event Action<string> OnRoomLeft;

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
            var payload = response.GetValue<PlayerPositionPayload>();

            OnPositionChanged?.Invoke(payload);
        });

        m_LobbySocket.OnUnityThread("player:movingChanged", response =>
        {
            var payload = response.GetValue<PlayerMovingPayload>();

            OnMovingChanged?.Invoke(payload);
        });

        m_LobbySocket.OnUnityThread("lobby:joined", response =>
        {
            var player = response.GetValue<LobbyPlayerData>(0);

            OnLobbyJoined?.Invoke(player);
        });

        m_LobbySocket.OnUnityThread("lobby:left", resposne =>
        {
            var userId = resposne.GetValue<string>(0);
            
            OnLobbyLeft?.Invoke(userId);
        });

        m_LobbySocket.OnUnityThread("player:skinChanged", response =>
        {
            var payload = response.GetValue<PlayerSkinPayload>();

            OnSkinChanged?.Invoke(payload);
        });

        m_LobbySocket.OnUnityThread("player:nicknameChanged", response =>
        {
            var payload = response.GetValue<PlayerNicknamePayload>();

            OnNicknameChanged?.Invoke(payload);
        });

        m_LobbySocket.OnUnityThread("player:readyChanged", response =>
        {
            var payload = response.GetValue<PlayerReadyPayload>();

            OnReadyChanged?.Invoke(payload);
        });

        m_LobbySocket.OnUnityThread("room:joined", response =>
        {
            var player = response.GetValue<RoomPlayerData>();

            OnRoomJoined?.Invoke(player);
        });

        m_LobbySocket.OnUnityThread("room:left", response =>
        {
            var userId = response.GetValue<string>();
            Debug.Log("left received");

            OnRoomLeft?.Invoke(userId);
        });

        m_LobbySocket.OnUnityThread("room:started", response =>
        {
            OnRoomStarted?.Invoke();
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
            var result = response.GetValue<NetworkResult<CreateRoomResponse>>();
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

    public void RequestLeaveRoom()
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("room:leave", (response) =>
        {
            var result = response.GetValue<NetworkResult<object>>();
            if (result.success)
            {
                context.Post(_ => OnLeaveRoomResponse?.Invoke(), null);
            }
            else
            {
                Debug.LogError($"Room leave failed: {result.error}");
            }
        });
    }

    public void RequestStartRoom()
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("room:start", (response) =>
        {
            var result = response.GetValue<NetworkResult<object>>();
            if (result.success)
            {
                context.Post(_ => OnStartRoomResponse?.Invoke(), null);
            }
            else
            {
                Debug.LogError($"Room start failed: {result.error}");
            }
        });
    }

    public void SendPlayerPosition(Position position)
    {
        m_LobbySocket.Emit("player:changePosition", position);
    }

    public void SendPlayerMoving(bool value)
    {
        m_LobbySocket.Emit("player:changeMoving", value);
    }

    public void SendPlayerSkin(int skinIndex)
    {
        m_LobbySocket.Emit("player:changeSkin", skinIndex);
    }

    public void SendPlayerNickname(string nickname)
    {
        m_LobbySocket.Emit("player:changeNickname", nickname);
    }

    public void SendPlayerReady(bool isReady)
    {
        m_LobbySocket.Emit("player:changeReady", isReady);
    }
}
