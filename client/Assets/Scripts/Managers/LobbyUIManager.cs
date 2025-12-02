using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] Canvas m_LobbyCanvas;
    [SerializeField] WardrobePanel m_WardrobePanel;
    [SerializeField] RoomListPanel m_RoomListPanel;
    [SerializeField] CurrentRoomPanel m_CurrentRoomPanel;

    public void ShowWardrobePanel()
    {
        m_WardrobePanel.Show();
    }

    public void HideWardrobePanel()
    {
        if (m_WardrobePanel)
        {
            m_WardrobePanel.Hide();
        }
    }

    public void ShowRoomListPanel(GameConfig config)
    {
        // m_RoomListPanel.SetGameConfig(config);
        m_RoomListPanel.Show();
    }
}
