using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ColorLabLifetimeScope : LifetimeScope
{
    bool m_Standalone = false;

    protected override void Awake()
    {
        Debug.Log(Parent);
        m_Standalone = Parent == null;
        Debug.Log(m_Standalone);

        base.Awake();
    }

    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log($"in configure => {m_Standalone}");
        if (m_Standalone)
        {
            Debug.Log("[ColorLab] Run in standalone mode");
            builder.RegisterComponentOnNewGameObject<MockAuthManager>(Lifetime.Scoped).As<IAuthManager>();
        }

        builder.Register<ColorLabNetworkService>(Lifetime.Singleton)
            .AsImplementedInterfaces().AsSelf();
        builder.RegisterComponentInHierarchy<ColorLabManager>();
    }
}
