using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcadeLab.Data;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using VContainer;

public class ColorLabNetworkService : INetworkService
{
    public event Action<ColorLabPlayerData[]> OnColorLabInitResponse;
    
    public event Action<int> OnCountdown;
    public event Action OnStartGame;

    public event Action<PlayerPositionPayload> OnPositionChanged;
    public event Action<PlayerMovingPayload> OnMovingChanged;

    [Inject] AuthManager m_AuthManager;

    SocketIOUnity m_ColorLabSocket;

    public void Initialize()
    {
    }

    public void InitializeSocket(string roomId)
    {
        m_ColorLabSocket = new SocketIOUnity("http://localhost:3000/colorLab", new SocketIOOptions
        {
            Auth = new Dictionary<string, string>
            {
                { "userId", m_AuthManager.UserId },
                { "roomId", roomId }
            }
        });
        m_ColorLabSocket.JsonSerializer = new NewtonsoftJsonSerializer();
        RegisterEventListeners();
    }

    public void Dispose()
    {
        Disconnect();
    }

    public async Task ConnectAsync()
    {
        await m_ColorLabSocket.ConnectAsync();
    }

    public void Disconnect()
    {
        m_ColorLabSocket.Disconnect();
    }
    
    void RegisterEventListeners()
    {
         m_ColorLabSocket.OnUnityThread("player:positionChanged", response =>
        {
            var payload = response.GetValue<PlayerPositionPayload>();

            OnPositionChanged?.Invoke(payload);
        });

        m_ColorLabSocket.OnUnityThread("player:movingChanged", response =>
        {
            var payload = response.GetValue<PlayerMovingPayload>();

            OnMovingChanged?.Invoke(payload);
        });

        m_ColorLabSocket.OnUnityThread("game:countdown", response =>
        {
            var count = response.GetValue<int>();

            OnCountdown?.Invoke(count);
        });

        m_ColorLabSocket.OnUnityThread("game:start", response =>
        {
            OnStartGame?.Invoke(); 
        });
    }

    public void RequestColorLabInit()
    {
        var context = SynchronizationContext.Current;
        m_ColorLabSocket.Emit("game:init", (response) =>
        {
            var result = response.GetValue<NetworkResult<ColorLabPlayerData[]>>();
            if (result.success)
            {
                context.Post(_ => OnColorLabInitResponse?.Invoke(result.data), null);
            }
            else
            {
                Debug.LogError($"ColorLab init failed: {result.error}");
            }
        });
    }

    public void SendPlayerReady()
    {
        m_ColorLabSocket.Emit("game:ready");
    }

    public void SendPlayerPosition(Position position)
    {
        m_ColorLabSocket.Emit("player:changePosition", position);
    }

    public void SendPlayerMoving(bool value)
    {
        m_ColorLabSocket.Emit("player:changeMoving", value);
    }
}
