using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HorizontalMovement : MonoBehaviour
{
    [Header("Ground movement")]
    [Tooltip("Movement speed on the ground")]
    [SerializeField]
    private float m_GroundSpeed = 5f;

    [Header("Air movement")]
    [Tooltip("The Fox gains this much speed every frame when moving in the air")]
    [SerializeField]
    private float m_AirAcceleration = 3f;

    [Tooltip("If the Fox is travelling faster than this speed in the air (eg. being shot from a canon), holding down horizontal input won't make him go any faster")]
    [SerializeField]
    private float m_MaxSpeedInputThreshold = 5f;

    [Tooltip("How Air Acceleration changes from \n 0 speed (100% Air Acceleration) to \n MaxSpeedInputThreshold (0% Air Acceleration)")]
    [SerializeField]
    private AnimationCurve m_AirAccelerationCurve;
    private float MoveSpeed => m_Grounded.OnGround ? m_GroundSpeed : m_AirAcceleration;

    private Rigidbody2D rb;
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
            rb.AddForce(new Vector2(directionX * m_AirAcceleration * 40 * GetAirSpeedCoefficient(), 0));
        }
    }

    private float GetAirSpeedCoefficient()
    {
        float coefficient;

        if ((directionX > 0 && rb.velocity.x > 0) || (directionX < 0 && rb.velocity.x < 0))
        {
            // If we're holding down the same direction input as the direction we're travelling,
            // Figure out how much of the air speed should be given to the fox based on how fast we're going
            // If we're already travelling faster than the input threshold, give 0% of the air speed
            float speed = Mathf.Abs(rb.velocity.x);
            float clampedSpeed = Mathf.Clamp(speed, 0, m_MaxSpeedInputThreshold);
            coefficient = clampedSpeed / m_MaxSpeedInputThreshold;
        }
        else
        {
            // If we're trying to change direction, give 100% of the air speed
            // to allow the fox to start going the other way asap
            coefficient = 0;
        }

        // Returning just the 'coefficient' value means the speed will change linearly
        // Putting it on a curve will allow for a more nuance change if the designer so desires (ie. exponentially, etc.)
        return m_AirAccelerationCurve.Evaluate(coefficient);
    }

    private void PlayFootStepSound()
    {
        m_FootstepTimer -= Time.deltaTime * Mathf.Abs(directionX);
        if (m_FootstepTimer <= 0)
        {
            var footIndex = Random.Range(0, 2);
            var offset = 0.05f - Random.Range(0, 0.1f);
            AudioController.PlaySound(m_FootStepSounds[footIndex], 0.5f, 1 + offset, MixerGroup.SFX);

            m_FootstepTimer = m_FootstepInterval;
        }
    }
}
