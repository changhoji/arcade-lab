using VContainer;
using VContainer.Unity;

public class ColorLabLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ColorLabNetworkService>(Lifetime.Singleton)
            .AsImplementedInterfaces().AsSelf();
        builder.RegisterComponentInHierarchy<ColorLabManager>();
    }
}
