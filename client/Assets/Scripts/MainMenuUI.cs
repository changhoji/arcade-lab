using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class MainMenuUI : MonoBehaviour
{
    [Inject] NetworkManager m_NetworkManager;
    public NetworkManager NetworkManager => m_NetworkManager;

    public async void OnClickGuest()
    {
        await NetworkManager.SignInAnonymously();
    }
}
