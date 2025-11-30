using System.Collections.Generic;
using System.Threading.Tasks;
using ArcadeLab.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class LobbyBrowserManager : MonoBehaviour
{
    [Inject] LobbyNetworkService m_LobbyService;
    [Inject] AuthManager m_AuthManager;
    [Inject] LobbyBrowserPanel m_Panel;

    Dictionary<string, LobbyData> m_Lobbies = new();

    void Start()
    {
        m_Panel.OnClickCreate += CreateLobby;
        m_Panel.OnClickRefresh += GetLobbyList;
        m_Panel.OnClickJoin += JoinLobby;
        m_AuthManager.OnSignInSuccess += HandleSignInSuccess;
        m_LobbyService.OnLobbyListResponse += HandleLobbyListResponse;
        m_LobbyService.OnCreateLobbyResponse += HandleCreateLobbyResponse;
        m_LobbyService.OnJoinLobbyResponse += HandleJoinLobbyResponse;
    }

    void OnDestroy()
    {
        m_Panel.OnClickCreate -= CreateLobby;
        m_Panel.OnClickRefresh -= GetLobbyList;
        m_Panel.OnClickJoin -= JoinLobby;
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
        Debug.Log("join lobby request!");
        m_LobbyService.RequestJoinLobby(lobbyId);
    }

    async void HandleSignInSuccess()
    {
        m_LobbyService.InitializeSocket();
        await m_LobbyService.ConnectAsync();
        Debug.Log("Lobby socket connected");
        GetLobbyList();
    }

    void HandleLobbyListResponse(LobbyData[] lobbies)
    {
        Debug.Log($"lobby list length = {lobbies.Length}");
        m_Lobbies.Clear();
        foreach (var lobby in lobbies)
        {
            m_Lobbies.Add(lobby.lobbyId, lobby);
        }

        m_Panel.UpdateLobbies(lobbies);
    }

    void HandleCreateLobbyResponse(string lobbyId)
    {
        Debug.Log($"created lobby id = {lobbyId}");
        if (string.IsNullOrEmpty(lobbyId))
        {
            Debug.LogError("failed create lobby");
        }
        else
        {
            PlayerPrefs.SetString("LobbyId", lobbyId);
            SceneManager.LoadScene("Lobby");
        }
    }

    void HandleJoinLobbyResponse(string lobbyId)
    {
        Debug.Log($"join lobby: {lobbyId}");
        if (string.IsNullOrEmpty(lobbyId))
        {
            Debug.LogError("failed join lobby");
        }
        else
        {
            PlayerPrefs.SetString("LobbyId", lobbyId);
            SceneManager.LoadScene("Lobby");
        }
    }
}
