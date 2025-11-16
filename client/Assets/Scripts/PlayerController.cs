using System;
using Unity.Collections;
using UnityEngine;
using VContainer;

public class PlayerController : MonoBehaviour
{
    [Inject] LobbyManager m_LobbyManager;
    [SerializeField] float m_MoveSpeed = 5f;
    Rigidbody2D m_Rigidbody;
    Animator m_Animator;
    SpriteRenderer m_SpriteRenderer;
    Vector2 m_MoveInput;

    public string UserId;
    public bool IsOwner = false;

    float m_LastSendTime;
    Vector3 m_PreviousPosition;

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
        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_LastSendTime = Time.time;
        m_PreviousPosition = transform.position;
    }

    void Start()
    {
        if (!IsOwner)
        {
            m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        m_LobbyManager = FindAnyObjectByType<LobbyManager>();
    }

    void Update()
    {
        if (!IsOwner)
        {
            if(transform.position != m_PreviousPosition)
            {
                m_Animator.SetBool("Walking", true);
                m_SpriteRenderer.flipX = transform.position.x < m_PreviousPosition.x;
            }
            else
            {
                m_Animator.SetBool("Walking", false);
            }
            m_PreviousPosition = transform.position;
            return;
        }

        m_MoveInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (m_MoveInput != Vector2.zero && Time.time - m_LastSendTime > 0.01f)
        {
            m_LastSendTime = Time.time;
            SendPosition();
            m_Animator.SetBool("Walking", true);
        }
        else
        {
            m_Animator.SetBool("Walking", false);
        }
    }

    void FixedUpdate()
    {
        m_Rigidbody.linearVelocity = m_MoveInput * m_MoveSpeed;
        if (m_Rigidbody.linearVelocityX != 0)
        {
            m_SpriteRenderer.flipX = m_Rigidbody.linearVelocityX < 0;    
        }
        
    }

    void SendPosition()
    {
        Position pos = new Position();
        pos.x = transform.position.x;
        pos.y = transform.position.y;
        m_LobbyManager.EmitPlayerMove(pos);
    }
}
