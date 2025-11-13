using System;
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
    SocketIOUnity m_Socket;
    public string UserId;

    public event Action<string> OnSignInSuccess;
    public event Action<LobbyPlayer[]> OnOtherPlayersReceived;
    public event Action<PlayerMovedData> OnPlayerMoved;
    public LobbyPlayer[] CachedOtherPlayers = null;

    public void Initialize()
    {
        Debug.Log("Initialize NetworkManager");
        DontDestroyOnLoad(gameObject);
        m_Socket = new SocketIOUnity("http://localhost:3000");
        m_Socket.JsonSerializer = new NewtonsoftJsonSerializer();


        RegisterEventListeners();
    }

    public async Task<bool> SignInAnonymously()
    {
        await m_Socket.ConnectAsync();
        Debug.Log("Requesting sign in anonymously...");
        await m_Socket.EmitAsync("signin:guest");

        return true;
    }

    void RegisterEventListeners()
    {
        m_Socket.On("signin:success", response =>
        {
            UserId = response.GetValue<string>(0);
            Debug.Log($"signin success! id = {UserId}");
            OnSignInSuccess?.Invoke(UserId);
        });

        m_Socket.On("player:other", response =>
        {
            try
            {
                CachedOtherPlayers = response.GetValue<LobbyPlayer[]>();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }

            OnOtherPlayersReceived?.Invoke(CachedOtherPlayers);
        });

        m_Socket.OnUnityThread("player:moved", response =>
        {
            var movedData = response.GetValue<PlayerMovedData>();
            OnPlayerMoved?.Invoke(movedData);
        });
    }

    public void EmitPlayerMove(float x, float y)
    {
        m_Socket.EmitAsync("player:move", new {
            x = x,
            y = y
        });
    }

}
