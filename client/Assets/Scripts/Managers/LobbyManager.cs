using System;
using System.Collections.Generic;
using ArcadeLab.Data;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

public class LobbyManager : MonoBehaviour
{
    public string LobbyId { get; private set; }

    public event Action<string, string> OnNicknameChanged;

    [SerializeField] GameObject m_PlayerPrefab;

    [Inject] IAuthManager m_AuthManager;
    [Inject] LobbyNetworkService m_LobbyService;
    Dictionary<string, PlayerBase> m_Players = new();

    void Start()
    {
        LobbyId = PlayerPrefs.GetString("LobbyId");

        m_LobbyService.OnLobbyInitResponse += HandleLobbyInitResponse;
        m_LobbyService.OnPositionChanged += HandlePositionChanged;
        m_LobbyService.OnMovingChanged += HandleMovingChanged;
        m_LobbyService.OnLobbyJoined += HandleLobbyJoined;
        m_LobbyService.OnLobbyLeft += HandleLobbyLeft;
        m_LobbyService.OnSkinChanged += HandleSkinChanged;
        m_LobbyService.OnNicknameChanged += HandleNicknameChanged;

        m_LobbyService.RequestLobbyInit();
    }

    void OnDestroy()
    {
        m_LobbyService.OnLobbyInitResponse -= HandleLobbyInitResponse;
        m_LobbyService.OnPositionChanged -= HandlePositionChanged;
        m_LobbyService.OnMovingChanged -= HandleMovingChanged;
        m_LobbyService.OnLobbyJoined -= HandleLobbyJoined;
        m_LobbyService.OnLobbyLeft -= HandleLobbyLeft;
        m_LobbyService.OnSkinChanged -= HandleSkinChanged;
        m_LobbyService.OnNicknameChanged -= HandleNicknameChanged;
    }

    void SpawnPlayer(LobbyPlayerData player)
    {
        var playerObject = Instantiate(m_PlayerPrefab, new Vector3(player.position.x, player.position.y, 0), Quaternion.identity);
        var isOwner = player.userId == m_AuthManager.UserId;

        var playerBase = playerObject.GetComponent<PlayerBase>();
        playerBase.Init(player.userId, player.nickname, player.skinIndex, isOwner);

        var playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerMovement.Init(player.position, player.isMoving);
        if (isOwner)
        {
            playerMovement.OnChangePosition += (position) => { m_LobbyService.SendPlayerPosition(position); };
            playerMovement.OnChangeIsMoving += (isMoving) => { m_LobbyService.SendPlayerMoving(isMoving); };
            playerBase.OnChangeSkin += (skinIndex) => { m_LobbyService.SendPlayerSkin(skinIndex); };
            playerBase.OnChangeNickname += (nickname) => { 
                m_LobbyService.SendPlayerNickname(nickname);
                OnNicknameChanged?.Invoke(player.userId, nickname);
            };
        }

        m_Players.Add(player.userId, playerBase);
    }

    void HandleLobbyInitResponse(LobbyPlayerData[] players)
    {
        foreach (var player in players)
        {
            SpawnPlayer(player);
        }
    }

    void HandleLobbyJoined(LobbyPlayerData player)
    {
        SpawnPlayer(player);
    }

    void HandleLobbyLeft(string userId)
    {
        if (!m_Players.ContainsKey(userId))
        {
            Debug.LogWarning("No player exists to remove");
            return;
        }

        var player = m_Players[userId];
        Destroy(player.gameObject);
        m_Players.Remove(userId);
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

    void HandleSkinChanged(PlayerSkinPayload payload)
    {
        string userId = payload.userId;
        int skinIndex = payload.skinIndex;

        if (!m_Players.ContainsKey(userId))
        {
            Debug.LogWarning("No player exists to modify skin");
            return;
        }

        var player = m_Players[userId];
        player.SetSkinIndex(skinIndex);
    }

    void HandleNicknameChanged(PlayerNicknamePayload payload)
    {
        string userId = payload.userId;
        string nickname = payload.nickname;

        if (!m_Players.ContainsKey(userId))
        {
            Debug.LogWarning("No player exists to modify nickname");
            return;
        }

        var player = m_Players[userId];
        player.SetNickname(nickname);

        OnNicknameChanged?.Invoke(userId, nickname);
    }
}
