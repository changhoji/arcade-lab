using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class ApplicationLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<AuthNetworkService>(Lifetime.Singleton)
            .AsImplementedInterfaces().AsSelf();
        builder.Register<LobbyNetworkService>(Lifetime.Singleton)
            .AsImplementedInterfaces().AsSelf();

        builder.RegisterComponentInHierarchy<AuthManager>();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
