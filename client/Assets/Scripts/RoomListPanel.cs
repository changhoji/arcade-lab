using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class RoomListPanel : MonoBehaviour
{
    [SerializeField] GameObject m_RoomItemPrefab;
    [SerializeField] Button m_CreateRoomButton;
    [SerializeField] Transform m_ScrollContent;
    RoomManager m_RoomManager;
    LobbyNetworkService m_LobbyService;
    List<RoomItem> m_RoomItems;

    
    [Inject]
    public void Construct(RoomManager roomManager, LobbyNetworkService lobbyService)
    {
        m_RoomManager = roomManager;
        m_LobbyService = lobbyService;
        m_RoomItems = new();
    }

    void Start()
    {
        m_CreateRoomButton.onClick.AddListener(OnCreateRoomClicked);
        // gameObject.SetActive(false);
    }

    public void Show(string gameId)
    {
        var rooms = m_RoomManager.GetRoomDatas(gameId);
        for (int i = 0; i < rooms.Length; i++)
        {
            var roomItem = Instantiate(
                m_RoomItemPrefab,
                m_ScrollContent
            ).GetComponent<RoomItem>();
            // roomItem.transform.local = new Vector3(0, -400 + 200*i, 0);
            roomItem.Initialize(rooms[i]);
            m_RoomItems.Add(roomItem);
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        foreach (var roomItem in m_RoomItems)
        {
            Destroy(roomItem.gameObject);
        }
        m_RoomItems.Clear();
        gameObject.SetActive(false);
    }

    void OnCreateRoomClicked()
    {
        m_RoomManager.CreateRoom("color-lab", "new room", 2);
    }
}
