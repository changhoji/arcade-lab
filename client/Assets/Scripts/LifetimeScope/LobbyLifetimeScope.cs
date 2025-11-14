using VContainer;
using VContainer.Unity;

public class MainLobbyLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<LobbyManager>();
    }
}
