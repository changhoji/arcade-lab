using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MainMenuLifetimeScope : LifetimeScope
{
    [SerializeField] AuthPanel m_AuthPanel;
    [SerializeField] LobbyBrowserManager m_LobbyBrowserManager;
    [SerializeField] LobbyBrowserPanel m_LobbyBrowserPanel;
    [SerializeField] MainMenuUIManager m_UIManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(m_UIManager);
        builder.RegisterComponent(m_AuthPanel)
            .AsImplementedInterfaces().AsSelf();

        builder.RegisterComponent(m_LobbyBrowserManager);
        builder.RegisterComponent(m_LobbyBrowserPanel)
            .AsImplementedInterfaces().AsSelf();
    }
}
