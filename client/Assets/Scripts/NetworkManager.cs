using System;
using System.Threading.Tasks;
using SocketIOClient;
using UnityEngine;
using VContainer.Unity;

public class NetworkManager : MonoBehaviour, IInitializable
{
    SocketIO m_Socket;
    string userId;

    public void Initialize()
    {
        Debug.Log("Initialize NetworkManager");
        DontDestroyOnLoad(gameObject);
        m_Socket = new SocketIO("http://localhost:3000");

        RegisterEventListeners();

        m_Socket.ConnectAsync();
    }


    public async Task<bool> SignInAnonymously()
    {
        Debug.Log("Requesting sign in anonymously...");
        await m_Socket.EmitAsync("signin:guest");

        return true;
    }

    void RegisterEventListeners()
    {
        m_Socket.On("signin:success", OnSignInSuccess);
    }

    void OnSignInSuccess(SocketIOResponse response)
    {
        userId = response.GetValue<string>(0);
        Debug.Log($"signin success! id = {userId}");
    }
}
