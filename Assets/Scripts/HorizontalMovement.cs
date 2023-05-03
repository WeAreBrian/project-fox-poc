using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalMovement : MonoBehaviour
{
    public float MoveSpeed => m_Grounded.OnGround ? GroundMoveSpeed : AirMoveSpeed;

    [Tooltip("Speed of the player when in the air")]
    public float AirMoveSpeed = 3f;
    public float MaxVelocityInputThreshold;
    public AnimationCurve CoefficientCurve;
    public float GroundMoveSpeed = 5;
    private Rigidbody2D rb;
    [SerializeField]
    private float directionX;
    private Grounded m_Grounded;
    private AnchorThrower m_Thrower;
    [SerializeField]
    private float m_FootstepInterval;
    private float m_FootstepTimer;
    private bool m_IsLeftFoot;
    [SerializeField]
    private List<AudioClip> m_FootStepSounds;
    
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
        if (horizontalAxisValue == 0) m_FootstepTimer = m_FootstepInterval;

        if (m_Grounded.OnGround)
        {
            rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);
            PlayFootStepSound();
        }
        else
        {
            rb.AddForce(new Vector2(directionX * AirMoveSpeed * 40 * GetAirCoefficient() , 0));
        }
    }

    private float GetAirCoefficient()
    {
        var coefficient = 0f;
        if ((directionX > 0 && rb.velocity.x > 0) || (directionX < 0 && rb.velocity.x < 0))
        {
            coefficient = (MaxVelocityInputThreshold-Mathf.Clamp(Mathf.Abs(rb.velocity.x), 0, MaxVelocityInputThreshold))/MaxVelocityInputThreshold;
        }
        else
        {
            coefficient = 1;
        }
        return CoefficientCurve.Evaluate(coefficient);
    }

    private void PlayFootStepSound()
    {
        m_FootstepTimer -= Time.deltaTime * Mathf.Abs(directionX);
        if (m_FootstepTimer <= 0)
        {
            var footIndex = Random.Range(0, 2);
            var offset = 0.05f - Random.Range(0, 0.1f);
            AudioController.PlaySound(m_FootStepSounds[footIndex], 0.25f, 1 + offset, MixerGroup.SFX);

            m_FootstepTimer = m_FootstepInterval;
        }
    }
}
