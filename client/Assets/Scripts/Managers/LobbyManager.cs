using System.Collections.Generic;
using System.Security.Cryptography;
using ArcadeLab.Data;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using VContainer;

public class LobbyManager : MonoBehaviour
{
    // [SerializeField] GameObject m_PlayerPrefab;

    // LobbyNetworkService m_LobbyService;
    // Dictionary<string, PlayerController> m_Players = new();

    // [Inject]
    // public void Construct(LobbyNetworkService lobbyService)
    // {
    //     m_LobbyService = lobbyService;
    //     m_LobbyService.OnPlayerConnected += HandlePlayerConnected;
    //     m_LobbyService.OnOtherPlayersReceived += OnOtherPlayersReceived;
    //     m_LobbyService.OnPlayerMoved += OnPlayerMoved;
    //     m_LobbyService.OnPlayerJoined += OnPlayerJoined;
    //     m_LobbyService.OnPlayerLeft += OnPlayerLeft;
    //     m_LobbyService.OnPlayerSkin += OnPlayerSkin;
    //     m_LobbyService.OnPlayerNickname += OnPlayerNickname;
    // }

    // async void Start()
    // {
    //     await m_LobbyService.ConnectAsync();
    // }

    // void HandlePlayerConnected(LobbyPlayerData player)
    // {
    //     SpawnPlayer(player, true);
    // }
    
    // void OnOtherPlayersReceived(LobbyPlayerData[] players)
    // {
    //     Debug.Log($"PlayerManager.OnOtherPlayersReceived: {players}");
    //     foreach (var player in players)
    //     {
    //         SpawnPlayer(player, false);
    //     }
    // }

    // void OnPlayerMoved(PlayerMoveData moveData)
    // {
    //     if (m_Players.TryGetValue(moveData.userId, out PlayerController pc))
    //     {
    //         pc.UpdateRemotePosition(moveData.position);
    //     }
    // }

    // void OnPlayerJoined(LobbyPlayerData player)
    // {
    //     SpawnPlayer(player, false);
    // }

    // void OnPlayerLeft(string userId)
    // {
    //     RemovePlayer(userId);
    // }

    // void OnPlayerSkin(PlayerSkinData skinData)
    // {
    //     if (m_Players.TryGetValue(skinData.userId, out var pc))
    //     {
    //         pc.SetSkinIndex(skinData.skinIndex);
    //     }
    // }

    // void OnPlayerNickname(PlayerNicknameData nicknameData)
    // {
    //     if (m_Players.TryGetValue(nicknameData.userId, out var pc))
    //     {
    //         pc.SetNickname(nicknameData.nickname);
    //     }
    // }

    // void SpawnPlayer(LobbyPlayerData player, bool isOwner)
    // {
    //     if (m_Players.ContainsKey(player.userId))
    //     {
    //         Debug.LogWarning($"Player {player.userId} is already exist");
    //         return;
    //     }

    //     var playerObject = Instantiate(m_PlayerPrefab, new Vector3(player.position.x, player.position.y), Quaternion.identity);
    //     var pc = playerObject.GetComponent<PlayerController>();
    //     pc.UserId = player.userId;
    //     pc.IsOwner = isOwner;
    //     pc.SetSkinIndex(player.skinIndex);
    //     pc.Nickname = player.nickname;

    //     if (pc.IsOwner)
    //     {
    //         pc.OnMoved += m_LobbyService.EmitPlayerMove;
    //         pc.OnSkinChanged += m_LobbyService.EmitPlayerSkin;
    //         pc.OnNicknameChanged += m_LobbyService.EmitPlayerNickname;
    //     }

    //     m_Players.Add(player.userId, pc);
    // }

    // void RemovePlayer(string userId)
    // {
    //     if (m_Players.TryGetValue(userId, out var pc))
    //     {
    //         if (pc.IsOwner)
    //         {
    //             pc.OnMoved -= m_LobbyService.EmitPlayerMove;
    //             pc.OnSkinChanged -= m_LobbyService.EmitPlayerSkin;
    //             pc.OnNicknameChanged -= m_LobbyService.EmitPlayerNickname;
    //         }
    //         Destroy(pc.gameObject);
    //         m_Players.Remove(userId);
    //     }
    // }

    // void OnDestroy()
    // {
    //     if (m_LobbyService != null)
    //     {
    //         m_LobbyService.OnOtherPlayersReceived -= OnOtherPlayersReceived;
    //         m_LobbyService.OnPlayerMoved -= OnPlayerMoved;
    //         m_LobbyService.OnPlayerJoined -= OnPlayerJoined;
    //         m_LobbyService.OnPlayerLeft -= OnPlayerLeft;
    //         m_LobbyService.OnPlayerSkin -= OnPlayerSkin;
    //         m_LobbyService.OnPlayerConnected -= HandlePlayerConnected;
    //         m_LobbyService.OnPlayerNickname -= OnPlayerNickname;

    //         m_LobbyService.Disconnect();
    //     }
    // }
}
