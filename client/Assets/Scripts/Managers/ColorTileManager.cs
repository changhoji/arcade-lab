using System.Collections.Generic;
using ArcadeLab.Data;
using UnityEngine;
using VContainer;

public class ColorTileManager : MonoBehaviour
{
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

        m_ColorLabService.OnStepTileResponse += HandleStepTileResponse;
        m_ColorLabService.OnStepperChanged += HandleStepperChanged;
    }

    void OnDestroy()
    {
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
                colorTile.OnPressed += () => { m_ColorLabService.RequestStepTile(gridPos); };
                colorTile.OnUnStepped += () => { m_ColorLabService.SendUnStepped(gridPos); };

                m_Tiles.Add(gridPos, tileObject.GetComponent<ColorTile>());
            }
        }
    }

    void HandleStepTileResponse(Vector2Int position)
    {
        m_Tiles[position].StepperId = m_AuthManager.UserId;
    }

    void HandleStepperChanged(TileStepperPayload payload)
    {
        Debug.Log($"payload stepperId: {payload.stepperId}");
        m_Tiles[payload.position].StepperId = payload.stepperId;
    }
}
