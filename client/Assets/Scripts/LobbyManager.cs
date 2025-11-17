using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using VContainer;

public class Position
{
    public float x;
    public float y;
}

public class LobbyPlayer
{
    public string userId;
    public Position position;
    public int skinIndex;
}

public class PlayerMovedData
{
    public string userId;
    public float x;
    public float y;
}

public class SkinData
{
    public string userId;
    public int skinIndex;
}

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

    public void EmitPlayerMove(Position position)
    {
        m_LobbyService.EmitPlayerMove(position);
    }

    public void EmitPlayerSkin(int skinIndex)
    {
        m_LobbyService.EmitPlayerSkin(skinIndex);
    }

    async void Start()
    {
        await m_LobbyService.ConnectLobby();
        SpawnPlayer(m_AuthManager.UserId, Vector2.zero, true, skinIndex: 0);
    }

    void OnOtherPlayersReceived(LobbyPlayer[] players)
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

    void OnPlayerMoved(PlayerMovedData movedData)
    {
        Debug.Log($"PlayerManager.OnPlayerMoved: {movedData}");
        if (m_Players.TryGetValue(movedData.userId, out PlayerController pc))
        {
            pc.UpdateRemotePosition(movedData.x, movedData.y);
        }
    }

    void OnPlayerJoined(LobbyPlayer player)
    {
        Debug.Log("lobbyManager.OnPlayerJoined");
        SpawnPlayer(player.userId, new Vector2(player.position.x, player.position.y), false, player.skinIndex);
    }

    void OnPlayerLeft(string userId)
    {
        RemovePlayer(userId);
    }

    void OnPlayerSkin(SkinData skinData)
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

        m_Players.Add(userId, pc);
    }

    void RemovePlayer(string userId)
    {
        Debug.Log("LobbyManager.RemovePlayer");

        if (!m_Players.ContainsKey(userId))
        {
            Debug.LogWarning($"Player {userId} is already left");
            return;
        }

        if (m_Players.TryGetValue(userId, out var pc))
        {
            Debug.Log("destroy");
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
            m_LobbyService.DisconnectLobby();
        }
    }
}
