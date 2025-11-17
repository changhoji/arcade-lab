using VContainer;
using VContainer.Unity;

public class MainLobbyLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<LobbyManager>();
        builder.RegisterComponentInHierarchy<LobbyUIManager>();
        builder.Register<LobbyNetworkService>(Lifetime.Singleton)
            .AsImplementedInterfaces()
            .AsSelf();
    }
}
