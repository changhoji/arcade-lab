using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class AuthPanel : UIPanelBase
{
    [SerializeField] Button m_GuestButton;
    [Inject] AuthManager m_AuthManager;

    void Start()
    {
        m_GuestButton.onClick.AddListener(OnClickGuest);
    }

    void OnClickGuest()
    {
        m_AuthManager.SignInAnonymously();
    }
}
