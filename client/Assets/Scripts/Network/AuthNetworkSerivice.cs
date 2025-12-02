using System;
using System.Threading;
using System.Threading.Tasks;
using ArcadeLab.Data;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;

public class AuthNetworkService : INetworkService
{
    public event Action<PlayerBaseData> OnSignInResponse;

    SocketIOUnity m_AuthSocket;

    public void Initialize()
    {
        m_AuthSocket = new SocketIOUnity("http://localhost:3000/auth");
        m_AuthSocket.JsonSerializer = new NewtonsoftJsonSerializer();
        RegisterEventListeners();
    }

    public void Dispose()
    {
        Disconnect();
    }

    public void RegisterEventListeners() {}

    public async Task ConnectAsync()
    {
        await m_AuthSocket.ConnectAsync();
    }

    public void Disconnect()
    {
        m_AuthSocket.Disconnect();
    }

    public void SignInAnonymously()
    {
        if (!m_AuthSocket.Connected)
        {
            Debug.LogWarning("Not connected to server yet");
            return;
        }

        var context = SynchronizationContext.Current;
        m_AuthSocket.Emit("auth:guest", (response) =>
        {
            var result = response.GetValue<NetworkResult<PlayerBaseData>>();
            if (result.success)
            {
                context.Post(_ => OnSignInResponse?.Invoke(result.data), null);
            }
            else
            {
                Debug.LogError($"Signin failed: {result.error}");
            }
            
        });
    }
}
