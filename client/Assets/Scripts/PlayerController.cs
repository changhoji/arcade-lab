using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float m_MoveSpeed = 5f;
    Rigidbody2D m_Rigidbody;
    Vector2 m_MoveInput;

    public string UserId;
    public bool IsOwner = false;

    NetworkManager m_NetworkManager;

    float m_LastSendTime;

    public void UpdateRemotePosition(float x, float y)
    {
        Debug.Log($"updateposition: {UserId} -> ({x}, {y})");
        try
        {
            transform.position = new Vector3(x, y, 0);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"in UpdateRemotePosition, {e.Message}");
        }
        
        Debug.Log($"after udpateposition, ({transform.position.x}, {transform.position.y})");
    }

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_LastSendTime = Time.time;
    }

    void Start()
    {
        if (!IsOwner)
        {
            m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        m_NetworkManager = FindAnyObjectByType<NetworkManager>();
        Debug.Log(m_NetworkManager);
    }

    void Update()
    {
        if (!IsOwner) return;
        m_MoveInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (m_MoveInput != Vector2.zero && Time.time - m_LastSendTime > 0.02f)
        {
            m_LastSendTime = Time.time;
            SendPosition();
        }
    }

    void FixedUpdate()
    {
        m_Rigidbody.linearVelocity = m_MoveInput * m_MoveSpeed;
    }

    void SendPosition()
    {
        m_NetworkManager.EmitPlayerMove(transform.position.x, transform.position.y);
    }
}
