using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [SerializeField] protected KeyCode m_KeyCode;

    protected bool m_IsInRange = false;

    protected virtual void Update()
    {
        if (!m_IsInRange) return;

        if (Input.GetKeyDown(m_KeyCode))
        {
            Debug.Log("call interact");
            Interact();
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerBase>(out var player) && player.IsOwner)
        {
            m_IsInRange = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerBase>(out var player) && player.IsOwner)
        {
            m_IsInRange = false;
        }
    }

    protected abstract void Interact();
}
