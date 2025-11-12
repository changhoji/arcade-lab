using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class ApplicationLifetimeScope : LifetimeScope
{
    [SerializeField] NetworkManager m_NetworkManager;
    [SerializeField] ConnectionManager m_ConnectionManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(m_NetworkManager).AsImplementedInterfaces();
        builder.RegisterComponent(m_ConnectionManager);
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
