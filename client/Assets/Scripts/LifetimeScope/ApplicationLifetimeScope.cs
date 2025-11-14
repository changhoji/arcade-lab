using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class ApplicationLifetimeScope : LifetimeScope
{
    [SerializeField] AuthManager m_AuthManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(m_AuthManager);
        builder.Register<AuthNetworkSerivice>(Lifetime.Singleton)
            .AsImplementedInterfaces()
            .AsSelf();
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
