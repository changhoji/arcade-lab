using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class AuthPanel : UIPanelBase
{
    [SerializeField] Button m_GuestButton;
    [Inject] AuthManager m_Manager;

    void Start()
    {
        m_GuestButton.onClick.AddListener(OnClickGuest);
    }

    void OnClickGuest()
    {
        m_Manager.SignInAnonymously();
    }
}
