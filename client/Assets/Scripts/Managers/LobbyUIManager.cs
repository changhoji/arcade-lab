using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] Canvas m_LobbyCanvas;
    [SerializeField] WardrobePanel m_WardrobePanel;
    [SerializeField] RoomListPanel m_RoomListPanel;

    public void ShowWardrobePanel(PlayerController player)
    {
        m_WardrobePanel.Show(player);
    }

    public void HideWardrobePanel()
    {
        if (m_WardrobePanel)
        {
            m_WardrobePanel.Hide();
        }
    }

    public void ShowRoomListPanel(string gameId)
    {
        m_RoomListPanel.Show(gameId);
    }
}
