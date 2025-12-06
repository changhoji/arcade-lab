using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] WardrobePanel m_WardrobePanel;
    [SerializeField] RoomListPanel m_RoomListPanel;
    [SerializeField] CurrentRoomPanel m_CurrentRoomPanel;

    [Inject] RoomManager m_RoomManager;

    void Start()
    {
        m_RoomManager.OnCreateRoomResposne += (data) => ShowCurrentRoomPanel();
        m_RoomManager.OnJoinRoomResponse += (data) => ShowCurrentRoomPanel();
    }

    void OnDestroy()
    {
        m_RoomManager.OnCreateRoomResposne -= (data) => ShowCurrentRoomPanel();
        m_RoomManager.OnJoinRoomResponse -= (data) => ShowCurrentRoomPanel();
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
        m_RoomManager.GetRoomList("color-lab");

        if (m_RoomManager.IsInRoom)
        {
            m_CurrentRoomPanel.Show();
        }
    }

    void ShowCurrentRoomPanel()
    {
        m_RoomListPanel.Hide();
        m_CurrentRoomPanel.Show();
        m_RoomManager.GetRoomList("color-lab");
    }
}