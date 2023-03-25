using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class AnchorThrower : MonoBehaviour
{
    public float MinThrowSpeed = 5;
    public float MaxThrowSpeed = 8;
    public float WindUpTime = 0.5f;

    public float ThrowSpeed => Mathf.Lerp(MinThrowSpeed, MaxThrowSpeed, (Time.time - m_WindUpStartTime) / WindUpTime);
    public Vector2 ThrowVelocity => m_ThrowDirection * ThrowSpeed;

    [SerializeField]
    private AnchorTrajectory m_Trajectory;
    private Vector2 m_ThrowDirection;
    private float m_WindUpStartTime;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        var anchorInteractAction = playerInput.actions["AnchorInteract"];

        anchorInteractAction.started += OnAnchorInteractStarted;
        anchorInteractAction.performed += OnAnchorInteractPerformed;

        m_Trajectory.gameObject.SetActive(false);
    }

    private void OnAnchorInteractStarted(InputAction.CallbackContext context)
    {
        if (!(context.interaction is SlowTapInteraction))
        {
            return;
        }

        m_WindUpStartTime = Time.time;
        m_Trajectory.gameObject.SetActive(true);
    }

    private void OnAnchorInteractPerformed(InputAction.CallbackContext context)
    {
        if (!(context.interaction is SlowTapInteraction))
        {
            return;
        }

        Throw(ThrowVelocity);
        m_Trajectory.gameObject.SetActive(false);
    }

    private void OnAim(InputValue value)
    {
        m_ThrowDirection = value.Get<Vector2>();
    }

    private void Throw(Vector2 velocity)
    {

    }

    private void Update()
    {
        m_Trajectory.Velocity = ThrowVelocity;
    }
}
