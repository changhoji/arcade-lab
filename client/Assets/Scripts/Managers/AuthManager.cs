using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArcadeLab.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class AuthManager : MonoBehaviour
{
    public string UserId { get; private set; } = null;
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
    public PlayerBaseData Player { get; private set; }

    public event Action OnSignInSuccess;

    [Inject] AuthNetworkService m_AuthService;
    [Inject] SceneLoader m_SceneLoader;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        Debug.Log("AuthManager.Start");
        Debug.Log(m_AuthService);
        if (m_AuthService != null)
        {
            m_AuthService.OnSignInResponse += HandleSignInResponse;
            await m_AuthService.ConnectAsync();
            Debug.Log("asdf");
            StartCoroutine(m_SceneLoader.LoadSceneAsync("MainMenu"));
        }
    }

    void OnDestroy()
    {
        if (m_AuthService != null)
        {
            m_AuthService.OnSignInResponse -= HandleSignInResponse;
            m_AuthService.Disconnect();
        }
    }

    public void SignInAnonymously()
    {
        m_AuthService.SignInAnonymously();
    }

    public void SignInWithEmail(string email, string password)
    {
        throw new NotImplementedException();
    }

    public void SignOut()
    {
        if (IsAuthenticated)
        {
            UserId = null;
            SceneManager.LoadScene("MainMenu");
        }
    } 

    void HandleSignInResponse(PlayerBaseData playerData)
    {
        if (playerData != null)
        {
            UserId = playerData.userId;
            Player = playerData;
            Debug.Log("signin success");
            OnSignInSuccess?.Invoke();
        }
        else
        {
            Debug.LogWarning("signin failed");
        }
    }
}
