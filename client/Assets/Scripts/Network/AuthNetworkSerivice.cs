using System;
using System.Threading.Tasks;
using ArcadeLab.Data;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using VContainer.Unity;

public class AuthNetworkSerivice : INetworkService
{
    public event Action<PlayerBaseData> OnSignInSuccess;

    SocketIOUnity m_AuthSocket;

    public void Initialize()
    {
        m_AuthSocket = new SocketIOUnity("http://localhost:3000");
        m_AuthSocket.JsonSerializer = new NewtonsoftJsonSerializer();
        RegisterEventListeners();
    }

    public void RegisterEventListeners()
    {
        m_AuthSocket.OnUnityThread("signin:success", response =>
        {
            var userId = response.GetValue<string>(0);
            // OnSignInSuccess?.Invoke()   
        });
    }

    public async Task ConnectAsync()
    {
        await m_AuthSocket.ConnectAsync();
    }

    public void Disconnect()
    {
        m_AuthSocket.Disconnect();
    }

    public async Task SignInAnonymously()
    {
        if (!m_AuthSocket.Connected)
        {
            Debug.LogWarning("Not connected to server yet");
        }
        await m_AuthSocket.EmitAsync("signin:guest");
    }
}
