using System.Threading.Tasks;
using VContainer.Unity;

public interface INetworkService : IInitializable
{
    public void RegisterEventListeners();

    public Task ConnectAsync();
    public void Disconnect();
}
