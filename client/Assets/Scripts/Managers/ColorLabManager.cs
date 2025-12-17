using System.Collections.Generic;
using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class ColorLabManager : MonoBehaviour
{
    public string RoomId { get; private set; }

    [SerializeField] GameObject m_PlayerPrefab;

    [Inject] AuthManager m_AuthManager;
    [Inject] ColorLabNetworkService m_ColorLabService;

    Dictionary<string, PlayerBase> m_Players = new();

    async void Start()
    {
        m_ColorLabService.OnColorLabInitResponse += HandleColorLabInitResponse;
        m_ColorLabService.OnPositionChanged += HandlePositionChanged;
        m_ColorLabService.OnMovingChanged += HandleMovingChanged;
        m_ColorLabService.OnCountdown += HandleCountdown;
        m_ColorLabService.OnStartGame += HandleStartGame;

        RoomId = PlayerPrefs.GetString("RoomId");

        m_ColorLabService.InitializeSocket(RoomId);
        await m_ColorLabService.ConnectAsync();
        m_ColorLabService.RequestColorLabInit();
    }

    void OnDestroy()
    {
        m_ColorLabService.OnColorLabInitResponse -= HandleColorLabInitResponse;
        m_ColorLabService.OnPositionChanged -= HandlePositionChanged;
        m_ColorLabService.OnMovingChanged -= HandleMovingChanged;
        m_ColorLabService.OnCountdown -= HandleCountdown;
        m_ColorLabService.OnStartGame -= HandleStartGame;
    }

    void SpawnPlayer(ColorLabPlayerData player)
    {
        var playerObject = Instantiate(m_PlayerPrefab, new Vector3(player.position.x, player.position.y), Quaternion.identity);
        var isOwner = player.userId == m_AuthManager.UserId;

        var playerBase = playerObject.GetComponent<PlayerBase>();
        playerBase.Init(player.userId, player.nickname, player.skinIndex, isOwner);

        var playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerMovement.Init(player.position, player.isMoving);
        if (isOwner)
        {
            playerMovement.OnChangePosition += (position) => { m_ColorLabService.SendPlayerPosition(position); };
            playerMovement.OnChangeIsMoving += (isMoving) => { m_ColorLabService.SendPlayerMoving(isMoving); };
        }

        m_Players.Add(player.userId, playerBase);
    }

    void HandleColorLabInitResponse(ColorLabPlayerData[] players)
    {
        foreach (var player in players)
        {
            SpawnPlayer(player);
        }
        m_ColorLabService.SendPlayerReady();
    }

    void HandlePositionChanged(PlayerPositionPayload payload)
    {
        string userId = payload.userId;
        Position position = payload.position;

        if (!m_Players.ContainsKey(userId))
        {
            Debug.LogWarning("No player exists to modify position");
            return;
        }

        var player = m_Players[userId];
        var playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.SetPosition(position);
    }

    void HandleMovingChanged(PlayerMovingPayload payload)
    {
        string userId = payload.userId;
        bool isMoving = payload.isMoving;

        if (!m_Players.ContainsKey(userId))
        {
            Debug.LogWarning("No player exists to modify isMoving");
            return;
        }

        var player = m_Players[userId];
        var playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.IsMoving = isMoving;
    }

    void HandleCountdown(int count)
    {
        Debug.Log($"count = {count}");
    }

    void HandleStartGame()
    {
        Debug.Log("start!");
    }
}
