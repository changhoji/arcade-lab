using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArcadeLab.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class LobbyBrowserManager : MonoBehaviour
{
    public event Action<LobbyData[]> OnLobbyListResponse;

    [Inject] LobbyNetworkService m_LobbyService;
    [Inject] IAuthManager m_AuthManager;
    [Inject] SceneLoader m_SceneLoader;

    Dictionary<string, LobbyData> m_Lobbies = new();

    void Start()
    {
        m_AuthManager.OnSignInSuccess += HandleSignInSuccess;
        m_LobbyService.OnLobbyListResponse += HandleLobbyListResponse;
        m_LobbyService.OnCreateLobbyResponse += HandleCreateLobbyResponse;
        m_LobbyService.OnJoinLobbyResponse += HandleJoinLobbyResponse;
    }

    void OnDestroy()
    {
        m_AuthManager.OnSignInSuccess -= HandleSignInSuccess;
        m_LobbyService.OnLobbyListResponse -= HandleLobbyListResponse;
        m_LobbyService.OnCreateLobbyResponse -= HandleCreateLobbyResponse;
        m_LobbyService.OnJoinLobbyResponse -= HandleJoinLobbyResponse;
    }

    public void GetLobbyList()
    {
        m_LobbyService.RequestLobbyList();
    }

    public void CreateLobby(string name)
    {
        m_LobbyService.RequestCreateLobby(name);
    }

    public void JoinLobby(string lobbyId)
    {
        m_LobbyService.RequestJoinLobby(lobbyId);
    }

    async void HandleSignInSuccess()
    {
        m_LobbyService.InitializeSocket();
        await m_LobbyService.ConnectAsync();
        GetLobbyList();
    }

    void HandleLobbyListResponse(LobbyData[] lobbies)
    {
        m_Lobbies.Clear();
        foreach (var lobby in lobbies)
        {
            m_Lobbies.Add(lobby.lobbyId, lobby);
        }

        OnLobbyListResponse?.Invoke(lobbies);
    }

    void HandleCreateLobbyResponse(string lobbyId)
    {
        PlayerPrefs.SetString("LobbyId", lobbyId);
        StartCoroutine(m_SceneLoader.LoadSceneAsync("Lobby"));
    }

    void HandleJoinLobbyResponse(string lobbyId)
    {
        PlayerPrefs.SetString("LobbyId", lobbyId);
        StartCoroutine(m_SceneLoader.LoadSceneAsync("Lobby"));
    }
}
