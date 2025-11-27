using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class MainMenuUI : MonoBehaviour
{
    [Inject] AuthManager m_AuthManager;

    public void OnClickGuest()
    {
        m_AuthManager.SignInAnonymously();
    }
}
