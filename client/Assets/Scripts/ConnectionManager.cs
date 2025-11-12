using UnityEngine;
using VContainer;

public class ConnectionManager : MonoBehaviour
{
    [Inject] NetworkManager m_NetworkManager;
    public NetworkManager NetworkManager => m_NetworkManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
