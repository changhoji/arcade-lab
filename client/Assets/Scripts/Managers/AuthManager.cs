using System;
using System.Threading.Tasks;
using ArcadeLab.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class AuthManager : MonoBehaviour
{
    public string UserId { get; private set; } = null;
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);

    AuthNetworkSerivice m_AuthSerivce;
    PlayerBaseManager m_PlayerManager;

    [Inject]
    public void Construct(AuthNetworkSerivice authService, PlayerBaseManager playerManager)
    {
        m_AuthSerivce = authService;
        m_PlayerManager = playerManager;

        m_AuthSerivce.OnSignInSuccess += HandleSignInSuccess;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        if (m_AuthSerivce != null)
        {
            await m_AuthSerivce.ConnectAsync();
        }
    }

    void OnDestroy()
    {
        if (m_AuthSerivce != null)
        {
            m_AuthSerivce.OnSignInSuccess -= HandleSignInSuccess;
        }
    }

    public async Task SignInAnonymously()
    {
        await m_AuthSerivce.SignInAnonymously();
    }

    public async Task SignInWithEmail(string email, string password)
    {
        throw new NotImplementedException();
    }

    public void SignOut()
    {
        if (IsAuthenticated)
        {
            UserId = null;
            m_AuthSerivce.Disconnect();
            SceneManager.LoadScene("MainMenu");
        }
    } 

    void HandleSignInSuccess(PlayerBaseData player)
    {
        m_PlayerManager.AddPlayer(player);

        SceneManager.LoadScene("Lobby");
    }
}
