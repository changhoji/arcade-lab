using UnityEngine;
using VContainer;

public class Wardrobe : InteractableBase
{
    LobbyUIManager m_LobbyUIManager;

    void Start()
    {
        m_LobbyUIManager = FindAnyObjectByType<LobbyUIManager>();
    }

    protected override void Interact()
    {
        m_LobbyUIManager.ShowWardrobePanel();
    }
}
