using System;
using System.Collections.Generic;
using ArcadeLab.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class RoomListPanel : UIPanelBase
{
    public event Action<string> OnClickRefresh; // gameId
    public event Action<string, string> OnClickCreate; // gameId, name
    public event Action<string> OnClickJoin;

    [SerializeField] Transform m_ScrollContent;
    [SerializeField] GameObject m_RoomItemPrefab;
    [SerializeField] TMP_InputField m_NameInput;
    [SerializeField] Button m_CreateButton;
    [SerializeField] Button m_RefreshButton;

    List<RoomItem> m_RoomItems = new();
    GameConfig m_GameConfig;

    void Start()
    {
        m_CreateButton.onClick.AddListener(() => OnClickCreate?.Invoke(m_GameConfig.gameId, m_NameInput.text));
        m_RefreshButton.onClick.AddListener(() => OnClickRefresh?.Invoke(m_GameConfig.gameId));
        gameObject.SetActive(false);
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
            roomItem.OnClickJoin += (roomId) => OnClickJoin?.Invoke(roomId);
            roomItem.transform.Translate(new Vector3(0, -50*i, 0));
            m_RoomItems.Add(roomItem);
        }
    }

    public void SetGameConfig(GameConfig config)
    {
        m_GameConfig = config;
    }

    // public override void Show()
    // {
    //     base.Show();

    //     var rooms = m_RoomManager.GetRoomDatas(m_GameConfig.gameId);
    //     for (int i = 0; i < rooms.Length; i++)
    //     {
    //         var roomItem = Instantiate(
    //             m_RoomItemPrefab,
    //             m_ScrollContent
    //         ).GetComponent<RoomItem>();
    //         roomItem.Initialize(rooms[i], m_GameConfig);
    //         m_RoomItems.Add(roomItem);
    //     }
    // }

    // public override void Hide()
    // {
    //     foreach (var roomItem in m_RoomItems)
    //     {
    //         Destroy(roomItem.gameObject);
    //     }
    //     m_RoomItems.Clear();

    //     base.Hide();
    // }

    // void OnCreateRoomClicked()
    // {
    //     m_RoomManager.CreateRoom(m_GameConfig.gameId, "new room", 2);
    // }
}
