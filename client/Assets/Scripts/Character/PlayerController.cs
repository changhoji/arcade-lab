using System;
using ArcadeLab.Data;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using VContainer;

public class PlayerController : MonoBehaviour
{
    public static PlayerController LocalPlayer { get; private set; }
    public event Action<Position> OnMoved;
    public event Action<int> OnSkinChanged;
    public event Action<string> OnNicknameChanged;

    public string UserId;
    public bool IsOwner = false;
    public string Nickname = "";

    [SerializeField] float m_MoveSpeed = 5f;
    [SerializeField] PlayerLibrary m_PlayerLibrary;
    [SerializeField] TextMeshPro m_NicknameText;

    Rigidbody2D m_Rigidbody;
    Animator m_Animator;
    SpriteLibrary m_SpriteLibrary;
    SpriteRenderer m_SpriteRenderer;

    Vector2 m_MoveInput;
    float m_LastSendTime;
    Vector3 m_PreviousPosition;
    bool m_IsMovable = true;

    public void UpdateRemotePosition(Position position)
    {
        transform.position = new Vector3(position.x, position.y);
    }

    public void SetIsMovable(bool value)
    {
        m_IsMovable = value;
    }

    public void SetSkinIndex(int index)
    {
        m_SpriteLibrary.spriteLibraryAsset = m_PlayerLibrary.Library[index];
        if (IsOwner)
        {
            OnSkinChanged?.Invoke(index);
        }
    }

    public void SetNickname(string nickname)
    {
        m_NicknameText.text = nickname;
        if (IsOwner)
        {
            OnNicknameChanged?.Invoke(nickname);
        }
    }

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_SpriteLibrary = GetComponent<SpriteLibrary>();
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
        else
        {
            LocalPlayer = this;
        }
        m_NicknameText.text = Nickname;
    }

    void Update()
    {
        if (!IsOwner)
        {
            if(transform.position != m_PreviousPosition)
            {
                m_Animator.SetBool("IsMoving", true);
                m_SpriteRenderer.flipX = transform.position.x < m_PreviousPosition.x;
            }
            else
            {
                m_Animator.SetBool("IsMoving", false);
            }
            m_PreviousPosition = transform.position;
            return;
        }

        if (!m_IsMovable) return;
        m_MoveInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (m_MoveInput != Vector2.zero && Time.time - m_LastSendTime > 0.01f)
        {
            m_LastSendTime = Time.time;
            Position pos = new Position(transform);
            OnMoved?.Invoke(pos);
            m_Animator.SetBool("IsMoving", true);
        }
        else
        {
            m_Animator.SetBool("IsMoving", false);
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
}
