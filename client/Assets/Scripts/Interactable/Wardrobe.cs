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
        Debug.Log("show wardrobe panel");
        m_LobbyUIManager.ShowWardrobePanel();
    }
}
