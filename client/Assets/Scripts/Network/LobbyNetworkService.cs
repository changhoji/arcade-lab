using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcadeLab.Data;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class LobbyNetworkService : INetworkService
{

    // #region Player Events
    // public event Action<LobbyPlayerData> OnPlayerConnected;
    // public event Action<LobbyPlayerData[]> OnOtherPlayersReceived;
    // public event Action<PlayerMoveData> OnPlayerMoved;
    // public event Action<LobbyPlayerData> OnPlayerJoined;
    // public event Action<string> OnPlayerLeft;
    // public event Action<PlayerSkinData> OnPlayerSkin;
    // public event Action<PlayerNicknameData> OnPlayerNickname;
    // #endregion

    // #region Room Events
    // public event Action<RoomData> OnRoomCreated;
    // public event Action<string> OnRoomDeleted;
    // #endregion
    public event Action<LobbyData[]> OnLobbyListResponse;
    public event Action<string> OnCreateLobbyResponse;
    public event Action<string> OnJoinLobbyResponse;


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
        // RegisterPlayerEventListeners();
        // RegisterRoomEventListeners();
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
            var lobbies = response.GetValue<LobbyData[]>(0);
            context.Post(_ => OnLobbyListResponse?.Invoke(lobbies), null);
        });
    }

    public void RequestCreateLobby(string name)
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("lobby:create", (response) =>
        {
            var lobbyId = response.GetValue<string>(0);
            context.Post(_ => OnCreateLobbyResponse?.Invoke(lobbyId), null);
        }, name);
    }

    public void RequestJoinLobby(string lobbyId)
    {
        var context = SynchronizationContext.Current;
        m_LobbySocket.Emit("lobby:join", (response) =>
        {
            var lobbyId = response.GetValue<string>(0);
            context.Post(_ => OnJoinLobbyResponse?.Invoke(lobbyId), null);
        }, lobbyId);
    }

    // void RegisterPlayerEventListeners()
    // {
    //     m_LobbySocket.OnUnityThread("player:connect", response =>
    //     {
    //         var player = response.GetValue<LobbyPlayerData>();
    //         OnPlayerConnected?.Invoke(player);
    //     });

    //     m_LobbySocket.OnUnityThread("player:others", response =>
    //     {
    //         var others = response.GetValue<LobbyPlayerData[]>();
    //         OnOtherPlayersReceived?.Invoke(others);
    //     });

    //     m_LobbySocket.OnUnityThread("player:move", response =>
    //     {
    //         var movedData = response.GetValue<PlayerMoveData>();
    //         OnPlayerMoved?.Invoke(movedData);
    //     });

    //     m_LobbySocket.OnUnityThread("player:join", response =>
    //     {
    //         Debug.Log("joined");
    //         var joinedPlayer = response.GetValue<LobbyPlayerData>();
    //         OnPlayerJoined?.Invoke(joinedPlayer);
    //     });

    //     m_LobbySocket.OnUnityThread("player:leave", response =>
    //     {
    //         Debug.Log("left");
    //         var leftId = response.GetValue<string>(0);
    //         Debug.Log(leftId);
    //         OnPlayerLeft?.Invoke(leftId);
    //     });

    //     m_LobbySocket.OnUnityThread("player:skin", response =>
    //     {
    //         var skinData = response.GetValue<PlayerSkinData>();
    //         OnPlayerSkin?.Invoke(skinData);
    //     });

    //     m_LobbySocket.OnUnityThread("player:nickname", response =>
    //     {
    //         var nicknameData = response.GetValue<PlayerNicknameData>();
    //         OnPlayerNickname?.Invoke(nicknameData);
    //     });
    // }

    // void RegisterRoomEventListeners()
    // {
    //     m_LobbySocket.OnUnityThread("room:create", response =>
    //     {
    //         var roomData = response.GetValue<RoomData>();
    //         OnRoomCreated?.Invoke(roomData);
    //     });
    // }


    // public void EmitPlayerMove(Position position)
    // {
    //     m_LobbySocket.Emit("player:move", position);
    // }

    // public void EmitPlayerSkin(int skinIndex)
    // {
    //     m_LobbySocket.Emit("player:skin", skinIndex);
    // }

    // public void EmitPlayerNickname(string nickname)
    // {
    //     m_LobbySocket.Emit("player:nickname", nickname);
    // }

    // public void EmitCreateRoom(string gameId, string roomName, int maxPlayers)
    // {
    //     var request = new RoomCreateData();
    //     request.gameId = gameId;
    //     request.roomName = roomName;
    //     request.maxPlayers = maxPlayers;
    //     m_LobbySocket.Emit("room:create", request);
    // }
}
