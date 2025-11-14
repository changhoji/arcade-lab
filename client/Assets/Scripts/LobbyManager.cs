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
}

public class PlayerMovedData
{
    public string userId;
    public float x;
    public float y;
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
    }

    public void EmitPlayerMove(Position position)
    {
        m_LobbyService.EmitPlayerMove(position);
    }

    async void Start()
    {
        await m_LobbyService.ConnectLobby();
        SpawnPlayer(m_AuthManager.UserId, Vector2.zero, true);
    }

    void OnOtherPlayersReceived(LobbyPlayer[] players)
    {
        Debug.Log($"PlayerManager.OnOtherPlayersReceived: {players}");
        foreach (var player in players)
        {
            SpawnPlayer(
                player.userId,
                new Vector2(player.position.x, player.position.y),
                isOwner: false
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

    void SpawnPlayer(string userId, Vector2 position, bool isOwner)
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

        m_Players.Add(userId, pc);
    }
}
