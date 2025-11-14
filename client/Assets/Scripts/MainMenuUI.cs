using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class MainMenuUI : MonoBehaviour
{
    [Inject] AuthManager m_AuthManager;

    public async void OnClickGuest()
    {
        await m_AuthManager.SignInAnonymously();
    }
}
