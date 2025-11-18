using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArcadeLab.Data;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using VContainer;

public class LobbyNetworkService : INetworkService
{
    public event Action<LobbyPlayerData> OnPlayerConnected;
    public event Action<LobbyPlayerData[]> OnOtherPlayersReceived;
    public event Action<PlayerMoveData> OnPlayerMoved;
    public event Action<LobbyPlayerData> OnPlayerJoined;
    public event Action<string> OnPlayerLeft;
    public event Action<PlayerSkinData> OnPlayerSkin;
    public event Action<PlayerNicknameData> OnPlayerNickname;

    [Inject] AuthManager m_AuthManager;
    SocketIOUnity m_LobbySocket;


    public void Initialize()
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

    public void RegisterEventListeners()
    {
        m_LobbySocket.OnUnityThread("player:connect", response =>
        {
            var player = response.GetValue<LobbyPlayerData>();
            OnPlayerConnected?.Invoke(player);
        });

        m_LobbySocket.OnUnityThread("player:others", response =>
        {
            var others = response.GetValue<LobbyPlayerData[]>();
            OnOtherPlayersReceived?.Invoke(others);
        });

        m_LobbySocket.OnUnityThread("player:move", response =>
        {
            var movedData = response.GetValue<PlayerMoveData>();
            OnPlayerMoved?.Invoke(movedData);
        });

        m_LobbySocket.OnUnityThread("player:join", response =>
        {
            Debug.Log("joined");
            var joinedPlayer = response.GetValue<LobbyPlayerData>();
            OnPlayerJoined?.Invoke(joinedPlayer);
        });

        m_LobbySocket.OnUnityThread("player:leave", response =>
        {
            Debug.Log("left");
            var leftId = response.GetValue<string>(0);
            Debug.Log(leftId);
            OnPlayerLeft?.Invoke(leftId);
        });

        m_LobbySocket.OnUnityThread("player:skin", response =>
        {
            var skinData = response.GetValue<PlayerSkinData>();
            OnPlayerSkin?.Invoke(skinData);
        });

        m_LobbySocket.OnUnityThread("player:nickname", response =>
        {
            var nicknameData = response.GetValue<PlayerNicknameData>();
            OnPlayerNickname?.Invoke(nicknameData);
        });
    }

    public async Task ConnectAsync()
    {
        await m_LobbySocket.ConnectAsync();
    }

    public void Disconnect()
    {
        m_LobbySocket.Disconnect();
    }

    public void EmitPlayerMove(Position position)
    {
        m_LobbySocket.Emit("player:move", position);
    }

    public void EmitPlayerSkin(int skinIndex)
    {
        m_LobbySocket.Emit("player:skin", skinIndex);
    }

    public void EmitPlayerNickname(string nickname)
    {
        m_LobbySocket.Emit("player:nickname", nickname);
    }
}
