using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using VContainer;

public class LobbyPlayerManager : MonoBehaviour
{
    [Inject] NetworkManager m_NetworkManager;
    [SerializeField] GameObject m_PlayerPrefab;
    Dictionary<string, PlayerController> m_Players = new();
    string m_UserId = "";

    async void Start()
    {
        Debug.Log("[PlayerManager] Start called");

        
        // 이벤트 구독
        m_NetworkManager.OnSignInSuccess += OnSignInSuccess;
        m_NetworkManager.OnOtherPlayersReceived += OnOtherPlayersReceived;
        m_NetworkManager.OnPlayerMoved += OnPlayerMoved;
        await m_NetworkManager.ConnectToMainLobby();
        
        Debug.Log("[PlayerManager] Events subscribed");
        
        // 이미 로그인됐는지 확인
        if (!string.IsNullOrEmpty(m_NetworkManager.UserId))
        {
            Debug.Log("[PlayerManager] Already signed in, spawning my player");
            OnSignInSuccess(m_NetworkManager.UserId);
        }
        
    }

    void OnSignInSuccess(string userId)
    {
        Debug.Log($"PlayerManager.OnSignInSuccess: {userId}");
        m_UserId = userId;
        SpawnPlayer(userId, Vector2.zero, true);
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
