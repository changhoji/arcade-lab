using System;
using ArcadeLab.Data;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public event Action<Position> OnChangePosition;
    public event Action<bool> OnChangeIsMoving;

    public bool IsMoving
    {
        get => m_IsMoving;
        set
        {
            m_IsMoving = value;
            m_Animator.SetBool("IsMoving", value);
            OnChangeIsMoving?.Invoke(value);
        }
    }

    public bool IsMovable
    {
        get => m_IsMovable;
        set => m_IsMovable = value;
    }

    [SerializeField] PlayerBase m_PlayerBase;
    [SerializeField] float m_MoveSpeed = 5f;

    Animator m_Animator;
    Rigidbody2D m_RigidBody;
    SpriteRenderer m_SpriteRenderer;
    
    Vector2 m_MoveInput;
    bool m_IsMoving = false;
    bool m_IsMovable = true;
    float m_PreviousX = 0;

    public void Init(Position position, bool isMoving)
    {
        SetPosition(position);
        IsMoving = isMoving;
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        m_RigidBody.bodyType = m_PlayerBase.IsOwner ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (transform.position.x != m_PreviousX)
        {
            m_SpriteRenderer.flipX = transform.position.x < m_PreviousX;
        }

        m_PreviousX = transform.position.x;

        if (!m_PlayerBase.IsOwner || !IsMovable)
        {
            return;
        }

        m_MoveInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (m_MoveInput != Vector2.zero)
        {
            if (!IsMoving) IsMoving = true;
            OnChangePosition?.Invoke(new Position(transform));
            m_Animator.SetBool("IsMoving", true);
        }
        else
        {
            if (IsMoving) IsMoving = false;
            m_Animator.SetBool("IsMoving", false);
        }
    }

    void FixedUpdate()
    {
        m_RigidBody.linearVelocity = m_MoveSpeed * m_MoveInput;
        if (m_RigidBody.linearVelocityX != 0)
        {
            m_SpriteRenderer.flipX = m_RigidBody.linearVelocityX < 0;
        }
    }

    public void SetPosition(Position position)
    {
        transform.position = new Vector2(position.x, position.y);
    }
}
