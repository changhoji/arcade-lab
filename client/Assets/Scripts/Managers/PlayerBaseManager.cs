using System.Collections.Generic;
using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class PlayerBaseManager : MonoBehaviour
{
    private Dictionary<string, PlayerBaseData> m_Players = new();

    public void AddPlayer(PlayerBaseData playerData)
    {
        if (m_Players.ContainsKey(playerData.userId))
        {
            Debug.LogError("This userid is already exist!");
            return;
        }

        m_Players.Add(playerData.userId, playerData);
    }

}
