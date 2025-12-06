using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class AuthPanel : UIPanelBase
{
    [Inject] AuthManager m_Manager;

    Button m_GuestButton;

    protected override void Awake()
    {
        base.Awake();

        m_GuestButton = m_Root.Q<Button>("login-button");
    }

    void Start()
    {
        m_GuestButton.clicked += OnClickGuest;
    }

    void OnDestroy()
    {
        m_GuestButton.clicked -= OnClickGuest;
    }

    void OnClickGuest()
    {
        m_Manager.SignInAnonymously();
    }
}
