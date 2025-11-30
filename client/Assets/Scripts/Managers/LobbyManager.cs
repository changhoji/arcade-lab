using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ArcadeLab.Data;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using VContainer;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject m_PlayerPrefab;

    [Inject] AuthManager m_AuthManager;
    [Inject] LobbyNetworkService m_LobbyService;
    Dictionary<string, PlayerBase> m_Players = new();

    public string LobbyId { get; private set; }

    void Start()
    {
        LobbyId = PlayerPrefs.GetString("LobbyId");
        m_LobbyService.OnLobbyInitResponse += HandleLobbyInitResponse;
        m_LobbyService.RequestLobbyInit();
        m_LobbyService.OnPlayerMoved += HandlePlayerMoved;
        m_LobbyService.OnPlayerMoving += HandlePlayerMoving;
        m_LobbyService.OnPlayerJoined += HandlePlayerJoined;
        m_LobbyService.OnPlayerLeft += HandlePlayerLeft;
        m_LobbyService.OnSkinChanged += HandleSkinChanged;
    }

    void OnDestroy()
    {
        m_LobbyService.OnLobbyInitResponse -= HandleLobbyInitResponse;
        m_LobbyService.OnPlayerMoved -= HandlePlayerMoved;
        m_LobbyService.OnPlayerMoving -= HandlePlayerMoving;
        m_LobbyService.OnPlayerJoined -= HandlePlayerJoined;
        m_LobbyService.OnPlayerLeft -= HandlePlayerLeft;
        m_LobbyService.OnSkinChanged -= HandleSkinChanged;
    }

    void HandleLobbyInitResponse(LobbyPlayerData[] players)
    {
        Debug.Log($"players length = {players.Length}");
        foreach (var player in players)
        {
            var playerObject = Instantiate(m_PlayerPrefab, new Vector3(player.position.x, player.position.y, 0), Quaternion.identity);
            var isOwner = player.userId == m_AuthManager.UserId;

            var playerBase = playerObject.GetComponent<PlayerBase>();
            playerBase.Init(player.userId, player.nickname, player.skinIndex, isOwner);

            var playerMovement = playerObject.GetComponent<PlayerMovement>();
            playerMovement.Init(player.position, player.isMoving);
            if (isOwner)
            {
                playerBase.OnChangeSkin += (skinIndex) => { m_LobbyService.EmitPlayerSkinIndex(skinIndex); };
                playerMovement.OnChangePosiiton += (position) => { m_LobbyService.EmitPlayerMoved(position); };
                playerMovement.OnStartMoving += () => { m_LobbyService.EmitPlayerMoving(true); };
                playerMovement.OnStopMoving += () => { m_LobbyService.EmitPlayerMoving(false); };
            }

            m_Players.Add(player.userId, playerBase);
        }
    }

    void HandlePlayerMoved(string userId, Position position)
    {
        var player = m_Players[userId];
        var playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.SetPosition(position);
    }

    void HandlePlayerMoving(string userId, bool isMoving)
    {
        Debug.Log("edit moving");
        var player = m_Players[userId];
        var playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.IsMoving = isMoving;
    }

    void HandlePlayerJoined(LobbyPlayerData player)
    {
        var playerObject = Instantiate(m_PlayerPrefab, new Vector3(player.position.x, player.position.y, 0), Quaternion.identity);
        var isOwner = player.userId == m_AuthManager.UserId;

        var playerBase = playerObject.GetComponent<PlayerBase>();
        playerBase.Init(player.userId, player.nickname, player.skinIndex, isOwner);

        var playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerMovement.Init(player.position, player.isMoving);

        m_Players.Add(player.userId, playerBase);
    }

    void HandlePlayerLeft(string userId)
    {
        if (m_Players.ContainsKey(userId))
        {
            var player = m_Players[userId];
            Destroy(player.gameObject);
            m_Players.Remove(userId);
        }
    }

    void HandleSkinChanged(string userId, int skinIndex)
    {
        if (m_Players.ContainsKey(userId))
        {
            var player = m_Players[userId];
            player.SetSkinIndex(skinIndex);
        }
    }
}
