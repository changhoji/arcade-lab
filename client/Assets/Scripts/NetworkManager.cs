using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEditor.Compilation;
using UnityEngine;
using VContainer.Unity;

public class Position
{
    public float x;
    public float y;
}

[System.Serializable]
public class PlayerMovedData
{
    public string userId;
    public float x;
    public float y;
}

[System.Serializable]
public class LobbyPlayer
{
    public string userId;
    public string socketId;
    public Position position;
}

public class NetworkManager : MonoBehaviour, IInitializable
{
    SocketIOUnity m_AuthSocket;
    SocketIOUnity m_LobbySocket;

    public string UserId;

    public event Action<string> OnSignInSuccess;
    public event Action<LobbyPlayer[]> OnOtherPlayersReceived;
    public event Action<PlayerMovedData> OnPlayerMoved;

    public void Initialize()
    {
        Debug.Log("Initialize NetworkManager");
        DontDestroyOnLoad(gameObject);

        m_AuthSocket = new SocketIOUnity("http://localhost:3000");
        m_AuthSocket.JsonSerializer = new NewtonsoftJsonSerializer();

        RegisterEventListeners();
    }

    public async Task<bool> SignInAnonymously()
    {
        await m_AuthSocket.ConnectAsync();
        Debug.Log("Requesting sign in anonymously...");
        await m_AuthSocket.EmitAsync("signin:guest");
        return true;
    }

    void RegisterEventListeners()
    {
        m_AuthSocket.OnUnityThread("signin:success", response =>
        {
            UserId = response.GetValue<string>(0);
            Debug.Log($"signin success! id = {UserId}");
            OnSignInSuccess?.Invoke(UserId);
        });
    }
    
    public async Task ConnectToLobby()
    {
        if (string.IsNullOrEmpty(UserId))
        {
            return;
        }

        if (m_LobbySocket != null)
        {
            return;
        }

        m_LobbySocket = new SocketIOUnity("http://localhost:3000/lobby", new SocketIOOptions
        {
            Auth = new Dictionary<string, string>
            {
                { "userId", UserId }
            }
        });
        m_LobbySocket.JsonSerializer = new NewtonsoftJsonSerializer();
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

        await m_LobbySocket.ConnectAsync();  
    }

    public void EmitPlayerMove(float x, float y)
    {
        m_LobbySocket.EmitAsync("player:move", new {
            x = x,
            y = y
        });
    }

}
