using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class RoomListPanel : UIPanelBase
{
    [SerializeField] GameObject m_RoomItemPrefab;
    [SerializeField] Button m_CreateRoomButton;
    [SerializeField] Transform m_ScrollContent;
    RoomManager m_RoomManager;
    List<RoomItem> m_RoomItems;
    GameConfig m_GameConfig;

    
    [Inject]
    public void Construct(RoomManager roomManager)
    {
        m_RoomManager = roomManager;
        m_RoomItems = new();
    }

    void Start()
    {
        m_CreateRoomButton.onClick.AddListener(OnCreateRoomClicked);
        gameObject.SetActive(false);
    }

    public void SetGameConfig(GameConfig config)
    {
        m_GameConfig = config;
    }

    public override void Show()
    {
        base.Show();

        var rooms = m_RoomManager.GetRoomDatas(m_GameConfig.gameId);
        for (int i = 0; i < rooms.Length; i++)
        {
            var roomItem = Instantiate(
                m_RoomItemPrefab,
                m_ScrollContent
            ).GetComponent<RoomItem>();
            roomItem.Initialize(rooms[i]);
            m_RoomItems.Add(roomItem);
        }
    }

    public override void Hide()
    {
        foreach (var roomItem in m_RoomItems)
        {
            Destroy(roomItem.gameObject);
        }
        m_RoomItems.Clear();

        base.Hide();
    }

    void OnCreateRoomClicked()
    {
        m_RoomManager.CreateRoom(m_GameConfig.gameId, "new room", 2);
    }
}
