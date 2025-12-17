using System;
using System.Threading.Tasks;
using VContainer.Unity;

public interface INetworkService : IInitializable, IDisposable
{
    public Task ConnectAsync();
    public void Disconnect();
}
