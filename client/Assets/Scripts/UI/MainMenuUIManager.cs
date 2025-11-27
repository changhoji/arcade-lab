using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class MainMenuUIManager : MonoBehaviour
{
    [Inject] AuthManager m_AuthManager;
    [Inject] AuthPanel m_AuthPanel;
    [Inject] LobbyBrowserPanel m_LobbyBrowserPanel;

    void Start()
    {
        m_AuthManager.OnSignInSuccess += ShowLobbyBrowserPanel;

        if (m_AuthManager.IsAuthenticated)
        {   
            ShowLobbyBrowserPanel();
        }
        else
        {
            ShowAuthPanel();
        }
    }

    void OnDestroy()
    {
        m_AuthManager.OnSignInSuccess -= ShowLobbyBrowserPanel;
    }

    public void ShowAuthPanel()
    {
        m_LobbyBrowserPanel.Hide();
        m_AuthPanel.Show();
    }

    public void ShowLobbyBrowserPanel()
    {
        Debug.Log("here");
        try
    {
        m_AuthPanel.Hide();
        Debug.Log("AuthPanel.Hide() completed");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error hiding AuthPanel: {e}");
    }
        Debug.Log("here");

        m_LobbyBrowserPanel.Show();
    }

    public void UpdateLobbies(LobbyData[] lobbies)
    {
        m_LobbyBrowserPanel.UpdateLobbies(lobbies);
    }
}
