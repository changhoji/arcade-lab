using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] Canvas m_LobbyCanvas;
    [SerializeField] WardrobePanel m_WardrobePanel;
    [SerializeField] RoomListPanel m_RoomListPanel;
    [Inject] CurrentRoomPanel m_CurrentRoomPanel;

    [Inject] RoomManager m_RoomManager;

    void Start()
    {
        m_RoomManager.OnCreateRoomResposne += HandleCreateRoomResponse;
    }

    void OnDestroy()
    {
        m_RoomManager.OnCreateRoomResposne -= HandleCreateRoomResponse;
    }

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
        m_RoomListPanel.SetGameConfig(config);
        m_RoomListPanel.Show();

        if (m_RoomManager.IsInRoom)
        {
            m_CurrentRoomPanel.Show();
        }
    }

    void HandleCreateRoomResponse(RoomData room)
    {
        if (room == null)
        {
            return;
        }

        m_CurrentRoomPanel.Show();
    }
}
