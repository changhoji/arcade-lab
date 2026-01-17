using System;
using UnityEngine;
using VContainer;

public class ColorTile : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    public string OwnerId
    {
        get => m_OwnerId;
        set
        {
            m_OwnerId = value;
            m_SpriteRenderer.color = value == m_AuthManager.UserId ? Color.blue : Color.red;
        }
    }
    public bool IsOccupied = false;

    public event Action OnStep;
    public event Action OnUnstep;

    IAuthManager m_AuthManager;
    SpriteRenderer m_SpriteRenderer;
    string m_OwnerId;

    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Vector2Int gridPosition, IAuthManager authManager)
    {
        GridPosition = gridPosition;
        m_AuthManager = authManager;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerBase>(out var player) && player.IsOwner)
        {
            Step();
            Debug.Log("call step()");
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.TryGetComponent<PlayerBase>(out var player) && player.IsOwner)
        {
            Unstep();
            Debug.Log("call unstep()");
        }  
    }

    void Step()
    {
        if (IsOccupied)
        {
            return;
        }

        OnStep?.Invoke();
    }

    void Unstep()
    {
        if (!IsOccupied || OwnerId != m_AuthManager.UserId)
        {
            return;
        }

        OnUnstep?.Invoke();
    }
}
