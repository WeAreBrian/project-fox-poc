using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class AnchorThrower : MonoBehaviour
{
    public float MinThrowSpeed = 8;
    public float MaxThrowSpeed = 15;
    [Range(0, 2)]
    public float WindUpTime = 0.5f;
    public AnimationCurve WindUpCurve;
    [Range(0, 2)]
    public float ThrowHoldTime = 0.2f;

    public bool WindingUp => m_Trajectory.gameObject.activeSelf;
    public float HoldTime => Time.time - m_WindUpStartTime;
    public float ThrowSpeed => Mathf.Lerp(MinThrowSpeed, MaxThrowSpeed, WindUpCurve.Evaluate(HoldTime / WindUpTime));
    public Vector2 ThrowVelocity => m_ThrowDirection * ThrowSpeed;

    [SerializeField]
    private AnchorTrajectory m_Trajectory;
    private Vector2 m_ThrowDirection;
    private float m_WindUpStartTime;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        var anchorInteractAction = playerInput.actions["AnchorInteract"];

        anchorInteractAction.started += _ => OnAnchorInteractStarted();
        anchorInteractAction.canceled += _ => OnAnchorInteractCanceled();

        m_Trajectory.gameObject.SetActive(false);
    }

    private void OnAnchorInteractStarted()
    {
        m_WindUpStartTime = Time.time;
        m_Trajectory.gameObject.SetActive(true);
    }

    private void OnAnchorInteractCanceled()
    {
        if (HoldTime > ThrowHoldTime)
        {
            ThrowAnchor(ThrowVelocity);
        }
        else
        {
            DropAnchor();
        }

        m_Trajectory.gameObject.SetActive(false);
    }

    private void OnAim(InputValue value)
    {
        var inputDirection = value.Get<Vector2>();

        if (Mathf.Approximately(inputDirection.sqrMagnitude, 0))
        {
            return;
        }

        m_ThrowDirection = inputDirection;
    }

    private void ThrowAnchor(Vector2 velocity)
    {
    }

    private void DropAnchor()
    {
    }

    private void Update()
    {
        m_Trajectory.Velocity = ThrowVelocity;
    }
}