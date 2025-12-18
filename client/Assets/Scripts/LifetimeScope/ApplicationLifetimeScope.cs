using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class ApplicationLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<SceneLoader>(Lifetime.Singleton);

        builder.Register<AuthNetworkService>(Lifetime.Singleton)
            .AsImplementedInterfaces().AsSelf();
        builder.Register<LobbyNetworkService>(Lifetime.Singleton)
            .AsImplementedInterfaces().AsSelf();

        builder.RegisterComponentOnNewGameObject<AuthManager>(Lifetime.Singleton).As<IAuthManager>();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // if (SceneManager.GetActiveScene().name == "Startup")
        // {
        //     SceneManager.LoadScene("MainMenu");
        // }
    }
}
