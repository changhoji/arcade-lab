using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] Canvas m_LobbyCanvas;
    [SerializeField] WardrobePanel m_WardrobePanel;

    public void ShowWardrobePanel(PlayerController player)
    {
        m_WardrobePanel.Show(player);
    }

    public void HideWardrobePanel()
    {
        m_WardrobePanel?.Hide();
    }
}
