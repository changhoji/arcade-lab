using Unity.VisualScripting;
using UnityEngine;
using VContainer;

public class Wardrobe : MonoBehaviour
{
    [Inject] LobbyUIManager m_LobbyUIManager;
    BoxCollider2D m_BoxCollider;
    PlayerController m_Player = null;
    bool m_IsUsable = false;

    void Awake()
    {
        m_BoxCollider = GetComponent<BoxCollider2D>();
    }
    
    void Start()
    {
        m_LobbyUIManager = FindAnyObjectByType<LobbyUIManager>();
    }

    void Update()
    {
        if (m_IsUsable && Input.GetKeyDown(KeyCode.F))
        {
            UseWardrobe();
        }

        if (m_IsUsable && Input.GetKeyDown(KeyCode.Escape))
        {
            m_LobbyUIManager.HideWardrobePanel();
        }

    }

    void UseWardrobe()
    {
        m_LobbyUIManager.ShowWardrobePanel(m_Player);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out var player) && player.IsOwner)
        {
            m_Player = player;
            m_IsUsable = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out var player) && player.IsOwner)
        {
            m_Player = null;
            m_IsUsable = false;
            m_LobbyUIManager.HideWardrobePanel();
        }
    }
}
