using UnityEngine;
using VContainer;

public class GamePortal : InteractableBase
{
    [SerializeField] protected GameConfig m_GameConfig;
    RoomListPanel m_RoomListPanel;
    LobbyUIManager m_LobbyUIManager;

    void Start()
    {
        m_RoomListPanel = FindAnyObjectByType<RoomListPanel>();
        m_LobbyUIManager = FindAnyObjectByType<LobbyUIManager>();
    }

    protected override void Interact()
    {
        m_RoomListPanel.SetGameConfig(m_GameConfig);
        m_LobbyUIManager.UseGamePortal(m_GameConfig);
    }
}
