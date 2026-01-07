using System;
using System.Collections.Generic;
using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class ColorTileManager : MonoBehaviour
{
    public event Action<string, int> OnScoreUpdated;
    [SerializeField] GameObject m_ColorTilePrefab;
    [SerializeField] int m_GridWidth = 20;
    [SerializeField] int m_GridHeight = 20;
    [SerializeField] float m_CellSize = .5f;

    [Inject] IAuthManager m_AuthManager;
    [Inject] ColorLabNetworkService m_ColorLabService;

    Dictionary<Vector2Int, ColorTile> m_Tiles = new();
    Dictionary<string, int> m_Scores = new();

    void Start()
    {
        GenerateTiles();

        m_ColorLabService.OnColorLabInitResponse += HandleColorLabInitResponse;
        m_ColorLabService.OnStepTileResponse += HandleStepTileResponse;
        m_ColorLabService.OnStepperChanged += HandleStepperChanged;
    }

    void OnDestroy()
    {
        m_ColorLabService.OnColorLabInitResponse -= HandleColorLabInitResponse;
        m_ColorLabService.OnStepTileResponse -= HandleStepTileResponse;
        m_ColorLabService.OnStepperChanged -= HandleStepperChanged;
    }

    void GenerateTiles()
    {
        for (int x = 0; x < m_GridWidth; x++)
        {
            for (int y = 0; y < m_GridHeight; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = new Vector3((-m_GridWidth/2f + x + .5f) * m_CellSize, (-m_GridHeight/2f + y + .5f) * m_CellSize, 0);

                var tileObject = Instantiate(m_ColorTilePrefab, worldPos, Quaternion.identity, transform);
                tileObject.transform.localScale = Vector3.one * m_CellSize;

                var colorTile = tileObject.GetComponent<ColorTile>();
                colorTile.Init(gridPos, m_AuthManager);
                colorTile.OnStep += () => { m_ColorLabService.RequestStepTile(gridPos); };
                colorTile.OnUnstep += () => { m_ColorLabService.SendUnStepped(gridPos); };

                m_Tiles.Add(gridPos, tileObject.GetComponent<ColorTile>());
            }
        }
    }

    void HandleColorLabInitResponse(ColorLabInitResponse response)
    {
        foreach (var player in response.players)
        {
            m_Scores[player.userId] = 0;
        }
    }

    void HandleStepTileResponse(Vector2Int position)
    {
        UpdateTile(position, m_AuthManager.UserId);
    }

    void HandleStepperChanged(TileStepperPayload payload)
    {
        UpdateTile(payload.position, payload.stepperId);
    }

    void UpdateTile(Vector2Int position, string stepperId)
    {
        string prevOwnerId = m_Tiles[position].OwnerId;
        
        if (stepperId == null)
        {
            m_Tiles[position].IsOccupied = false;
            return;
        }

        if (prevOwnerId != stepperId)
        {
            if (prevOwnerId != null)
            {
                m_Scores[prevOwnerId]--;
                OnScoreUpdated(prevOwnerId, m_Scores[prevOwnerId]);
            }
            m_Scores[stepperId]++;
            OnScoreUpdated(stepperId, m_Scores[stepperId]);
        }

        m_Tiles[position].IsOccupied = true;
        m_Tiles[position].OwnerId = stepperId;

        // if (prevStepperId != null)
        // {
        //     m_Scores[prevStepperId]--;
        //     OnScoreUpdated?.Invoke(prevStepperId, m_Scores[prevStepperId]);
        // }
        // m_Scores[stepperId]++;
        // OnScoreUpdated?.Invoke(stepperId, m_Scores[stepperId]);
    }
}
