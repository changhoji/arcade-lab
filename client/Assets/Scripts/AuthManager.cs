using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class AuthManager : MonoBehaviour
{
    public string UserId { get; private set; } = null;

    AuthNetworkSerivice m_AuthSerivce;

    [Inject]
    public void Construct(AuthNetworkSerivice authService)
    {
        m_AuthSerivce = authService;
        m_AuthSerivce.OnSignInSuccess += OnSignInSuccess;
        DontDestroyOnLoad(gameObject);
    }
    
    public async Task SignInAnonymously()
    {
        await m_AuthSerivce.SignInAnonymously();
    }

    void OnDestroy()
    {
        m_AuthSerivce.OnSignInSuccess -= OnSignInSuccess;
    }

    void OnSignInSuccess(string userId)
    {
        Debug.Log("OnSignInSuccess");
        UserId = userId;
        SceneManager.LoadScene("Lobby");
    }
}
