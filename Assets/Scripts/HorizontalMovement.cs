using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalMovement : MonoBehaviour
{
    [Tooltip("Speed of the player when in the air")]
    [SerializeField]
    private float m_GroundMoveSpeed = 5f;
    [SerializeField]
    private float m_AirAcceleration = 3f;
    [SerializeField]
    private float m_MaxAirSpeed;
    [SerializeField]
    private AnimationCurve m_AirAccelerationCurve;
    private float MoveSpeed => m_Grounded.OnGround ? m_GroundMoveSpeed : m_AirAcceleration;

    private Rigidbody2D rb;
    private float directionX;
    private Grounded m_Grounded;
    private AnchorThrower m_Thrower;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
        m_Thrower = GetComponent<AnchorThrower>();
    }
    
    private void OnMove(InputValue value)
    {
        directionX = value.Get<float>();
    }

    private void FixedUpdate()
    {
        if (m_Thrower.WindingUp)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        var horizontalAxisValue = directionX;

        if (m_Grounded.OnGround)
        {
            rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);
        }
        else
        {
            rb.AddForce(new Vector2(directionX * m_AirAcceleration * 40 * GetAirCoefficient() , 0));
        }
    }

    private float GetAirCoefficient()
    {
        var coefficient = 0f;
        if ((directionX > 0 && rb.velocity.x > 0) || (directionX < 0 && rb.velocity.x < 0))
        {
            coefficient = (m_MaxAirSpeed-Mathf.Clamp(Mathf.Abs(rb.velocity.x), 0, m_MaxAirSpeed))/m_MaxAirSpeed;
        }
        else
        {
            coefficient = 1;
        }
        return m_AirAccelerationCurve.Evaluate(coefficient);
    }
}
