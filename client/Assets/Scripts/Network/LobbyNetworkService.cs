using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using VContainer;

public class LobbyNetworkService : INetworkService
{
    private SocketIOUnity m_LobbySocket;

    public event Action<LobbyPlayer[]> OnOtherPlayersReceived;
    public event Action<PlayerMovedData> OnPlayerMoved;
    public event Action<LobbyPlayer> OnPlayerJoined;

    [Inject] AuthManager m_AuthManager;

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
        m_LobbySocket.OnUnityThread("player:others", response =>
        {
            var others = response.GetValue<LobbyPlayer[]>();
            OnOtherPlayersReceived?.Invoke(others);
        });

        m_LobbySocket.OnUnityThread("player:moved", response =>
        {
            var movedData = response.GetValue<PlayerMovedData>();
            OnPlayerMoved?.Invoke(movedData);
        });

        m_LobbySocket.OnUnityThread("player:joined", response =>
        {
            Debug.Log("joined");
            var joinedPlayer = response.GetValue<LobbyPlayer>();
            OnPlayerJoined?.Invoke(joinedPlayer);
        });
    }

    public async Task ConnectLobby()
    {
        await m_LobbySocket.ConnectAsync();
    }
    
    public void EmitPlayerMove(Position position)
    {
        m_LobbySocket.Emit("player:move", new
        {
            x = position.x,
            y = position.y
        });
    }
}
