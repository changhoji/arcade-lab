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
        m_RoomManager.OnCreateRoomResposne += (_) => ShowCurrentRoomPanel();
        m_RoomManager.OnJoinRoomResponse += (_) => ShowCurrentRoomPanel();
        m_CurrentRoomPanel.OnHidePanel += ShowRoomListPanel;
    }

    void OnDestroy()
    {
        m_RoomManager.OnCreateRoomResposne -= (_) => ShowCurrentRoomPanel();
        m_RoomManager.OnJoinRoomResponse -= (_) => ShowCurrentRoomPanel();
        m_CurrentRoomPanel.OnHidePanel -= ShowRoomListPanel;
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

    public void UseGamePortal(GameConfig config)
    {
        m_RoomListPanel.SetGameConfig(config);
        if (m_RoomManager.IsInRoom)
        {
            ShowCurrentRoomPanel();
        }
        else
        {
            ShowRoomListPanel();
        }
    }

    public void ShowRoomListPanel()
    {
        Debug.Log("show roomlist panel");
        m_RoomListPanel.Show();
    }

    void ShowCurrentRoomPanel()
    {
        m_RoomListPanel.Hide();
        m_CurrentRoomPanel.Show();
    }
}