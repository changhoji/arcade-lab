using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float m_MoveSpeed = 5f;
    Rigidbody2D m_Rigidbody;
    Vector2 m_MoveInput;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        m_MoveInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        m_Rigidbody.linearVelocity = m_MoveInput * m_MoveSpeed;
    }
}
