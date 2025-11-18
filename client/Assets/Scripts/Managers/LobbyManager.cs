using System.Collections.Generic;
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

    LobbyNetworkService m_LobbyService;
    Dictionary<string, PlayerController> m_Players = new();


    [Inject]
    public void Construct(LobbyNetworkService lobbyService)
    {
        m_LobbyService = lobbyService;
        m_LobbyService.OnOtherPlayersReceived += OnOtherPlayersReceived;
        m_LobbyService.OnPlayerMoved += OnPlayerMoved;
        m_LobbyService.OnPlayerJoined += OnPlayerJoined;
        m_LobbyService.OnPlayerLeft += OnPlayerLeft;
        m_LobbyService.OnPlayerSkin += OnPlayerSkin;
    }

    async void Start()
    {
        await m_LobbyService.ConnectAsync();
        SpawnPlayer(m_AuthManager.UserId, Vector2.zero, true, skinIndex: 0);
    }
    
    void OnOtherPlayersReceived(LobbyPlayerData[] players)
    {
        Debug.Log($"PlayerManager.OnOtherPlayersReceived: {players}");
        foreach (var player in players)
        {
            SpawnPlayer(
                player.userId,
                new Vector2(player.position.x, player.position.y),
                isOwner: false,
                skinIndex: player.skinIndex
            );
        }
    }

    void OnPlayerMoved(PlayerMoveData moveData)
    {
        if (m_Players.TryGetValue(moveData.userId, out PlayerController pc))
        {
            pc.UpdateRemotePosition(moveData.position);
        }
    }

    void OnPlayerJoined(LobbyPlayerData player)
    {
        SpawnPlayer(player.userId, new Vector2(player.position.x, player.position.y), false, player.skinIndex);
    }

    void OnPlayerLeft(string userId)
    {
        RemovePlayer(userId);
    }

    void OnPlayerSkin(PlayerSkinData skinData)
    {
        if (m_Players.TryGetValue(skinData.userId, out var pc))
        {
            pc.SetSkinIndex(skinData.skinIndex);
        }
    }

    void SpawnPlayer(string userId, Vector2 position, bool isOwner, int skinIndex)
    {
        if (m_Players.ContainsKey(userId))
        {
            Debug.LogWarning($"Player {userId} is already exist");
            return;
        }

        var player = Instantiate(m_PlayerPrefab, position, Quaternion.identity);
        var pc = player.GetComponent<PlayerController>();
        pc.UserId = userId;
        pc.IsOwner = isOwner;
        pc.SetSkinIndex(skinIndex);

        if (pc.IsOwner)
        {
            pc.OnMoved += m_LobbyService.EmitPlayerMove;
            pc.OnSkinChanged += m_LobbyService.EmitPlayerSkin;
        }

        m_Players.Add(userId, pc);
    }

    void RemovePlayer(string userId)
    {
        if (m_Players.TryGetValue(userId, out var pc))
        {
            if (pc.IsOwner)
            {
                pc.OnMoved -= m_LobbyService.EmitPlayerMove;
                pc.OnSkinChanged -= m_LobbyService.EmitPlayerSkin;
            }
            Destroy(pc.gameObject);
            m_Players.Remove(userId);
        }
    }

    void OnDestroy()
    {
        if (m_LobbyService != null)
        {
            m_LobbyService.OnOtherPlayersReceived -= OnOtherPlayersReceived;
            m_LobbyService.OnPlayerMoved -= OnPlayerMoved;
            m_LobbyService.OnPlayerJoined -= OnPlayerJoined;
            m_LobbyService.OnPlayerLeft -= OnPlayerLeft;
            m_LobbyService.Disconnect();
        }
    }
}
