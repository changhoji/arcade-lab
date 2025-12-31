using System;
using UnityEngine;
using VContainer;

public class ColorTile : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    public string StepperId
    {
        get => m_StepperId;
        set
        {
            Debug.Log($"change to {value}");
            m_StepperId = value;
            if (value != null)
            {
                m_SpriteRenderer.color = m_StepperId == m_AuthManager.UserId ? Color.blue : Color.red;    
            }
        }
    }

    public event Action OnPressed;
    public event Action OnUnStepped;

    IAuthManager m_AuthManager;
    SpriteRenderer m_SpriteRenderer;
    Color m_Color;
    string m_StepperId;

    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Color = Color.white;
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
            UnStep();
            Debug.Log("call unstep()");
        }  
    }


    void Step()
    {
        if (StepperId == null)
        {
            OnPressed?.Invoke();
        }
        
    }

    void UnStep()
    {
        if (StepperId == m_AuthManager.UserId)
        {
            OnUnStepped?.Invoke();
        }
    }
}
