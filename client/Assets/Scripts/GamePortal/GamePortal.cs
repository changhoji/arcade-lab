using UnityEngine;
using VContainer;

public class GamePortal : InteractableBase
{
    [SerializeField] protected GameConfig m_GameConfig;
    LobbyUIManager m_LobbyUIManager;

    void Start()
    {
        m_LobbyUIManager = FindAnyObjectByType<LobbyUIManager>();
    }

    protected override void Interact()
    {
        m_LobbyUIManager.ShowRoomListPanel(m_GameConfig);
    }
}
