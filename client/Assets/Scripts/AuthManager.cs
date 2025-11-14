using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class AuthManager : MonoBehaviour
{
    [Inject] NetworkManager m_NetworkManager;

    void Start()
    {
        m_NetworkManager.OnSignInSuccess += OnSignInSuccess;
    }

    void OnDestroy()
    {
        m_NetworkManager.OnSignInSuccess -= OnSignInSuccess;
    }
    
    void OnSignInSuccess(string userId)
    {
        Debug.Log("AuthManager.OnSignInSuccess");
        SceneManager.LoadScene("Lobby");
    }
}
