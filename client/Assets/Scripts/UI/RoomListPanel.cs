using System;
using System.Collections.Generic;
using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class RoomListPanel : UIPanelBase
{
    [SerializeField] Transform m_ScrollContent;
    [SerializeField] GameObject m_RoomItemPrefab;
    [SerializeField] TMP_InputField m_NameInput;
    [SerializeField] Button m_CreateButton;
    [SerializeField] Button m_RefreshButton;

    [Inject] RoomManager m_Manager;

    List<RoomItem> m_RoomItems = new();
    GameConfig m_GameConfig;

    void Start()
    {
        m_Manager.OnRoomListResponse += UpdateRooms;
        m_CreateButton.onClick.AddListener(() => m_Manager.CreateRoom(m_GameConfig.gameId, m_NameInput.text));
        m_RefreshButton.onClick.AddListener(() => m_Manager.JoinRoom(m_GameConfig.gameId));
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        m_Manager.OnRoomListResponse -= UpdateRooms;
    }

    public void UpdateRooms(RoomData[] rooms)
    {
        foreach (var roomItem in m_RoomItems)
        {
            Destroy(roomItem.gameObject);
        }
        m_RoomItems.Clear();

        for (int i = 0; i < rooms.Length; i++)
        {
            var roomId = rooms[i].roomId;
            var roomObject = Instantiate(m_RoomItemPrefab, m_ScrollContent);
            var roomItem = roomObject.GetComponent<RoomItem>();
            roomItem.Init(rooms[i], m_GameConfig);
            roomItem.OnClickJoin += (roomId) => m_Manager.JoinRoom(roomId);
            roomItem.transform.Translate(new Vector3(0, -50*i, 0));
            m_RoomItems.Add(roomItem);
        }
    }

    public void SetGameConfig(GameConfig config)
    {
        m_GameConfig = config;
    }
}
