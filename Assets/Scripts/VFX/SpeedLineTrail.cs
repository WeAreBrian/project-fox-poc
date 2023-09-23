using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLineTrail : MonoBehaviour
{
    private Rigidbody2D m_ParentRB;
    private float m_Speed;

    [Tooltip("Minimum speed it takes for the effect to start.")]
    [SerializeField]
    private float m_MinSpeedThreshold = 18.0f;

    [Tooltip("Maximum speed it takes for the effect to be at its maximum.")]
    [SerializeField]
    private float m_MaxSpeedThreshold = 30.0f;

    [Tooltip("How long it takes to smoothly reach the target trail lifetime/length when speed is increasing.")]
    [SerializeField]
    private float m_RisingDelay = 0f;

    [Tooltip("How long it takes to smoothly reach the target trail lifetime/length when speed is decreasing.")]
    [SerializeField]
    private float m_FallingFalling = 0.2f;

    [Tooltip("Maximum lifetime of the trail. This also looks like the 'length' but not really the length")]
    [SerializeField]
    private float m_MaxTrailLifetime = 1f;

    private float m_TargetTrailLifeTime = 1.0f;
    private float m_CurrentTrailLifeTime = 0.0f;
    private float m_Velocity = 0.0f;
    private TrailRenderer m_TrailRenderer;

    private void Awake()
    {
        // Get the Rigidbody2D component from the parent of the GameObject
        m_ParentRB = GetComponentInParent<Rigidbody2D>();
        m_TrailRenderer = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        // Update the speed based on the parent Rigidbody's velocity
        m_Speed = m_ParentRB.velocity.magnitude;

        // Adjust the target trail lifetime based on the current speed
        if (m_Speed >= m_MaxSpeedThreshold)
        {
            m_TargetTrailLifeTime = m_MaxTrailLifetime;
        }
        else if (m_Speed > m_MinSpeedThreshold)
        {
            m_TargetTrailLifeTime = m_MaxTrailLifetime * (m_Speed - m_MinSpeedThreshold) / (m_MaxSpeedThreshold - m_MinSpeedThreshold);
        }
        else if (m_Speed <= m_MinSpeedThreshold)
        {
            m_TargetTrailLifeTime = 0f;
        }

        // Determine the smoothing speed for the trail life transition (set smoothtime to either rising or falling var)
        float smoothTime = m_CurrentTrailLifeTime < m_TargetTrailLifeTime ? m_RisingDelay : m_FallingFalling;

        // Smoothly adjust the trail lifetime
        m_CurrentTrailLifeTime = Mathf.SmoothDamp(m_CurrentTrailLifeTime, m_TargetTrailLifeTime, ref m_Velocity, smoothTime);

        // Apply the current trail lifetime to the TrailRenderer component
        m_TrailRenderer.time = m_CurrentTrailLifeTime;
    }
}