using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSchemeTest : MonoBehaviour
{
    private PlayerInput m_PlayerInput;
    private InputAction m_AnchorInteract;

    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();

        m_AnchorInteract = m_PlayerInput.actions["AnchorInteract"];
    }

    private void OnMove(InputValue value)
    {
        var axis = value.Get<float>();

        if (axis > 0.1f)
        {
            Debug.Log($"Moving right");
        }
        else if (axis < -0.1f)
        {
            Debug.Log($"Moving left");
        }
    }

    private void OnJump()
    {
        Debug.Log("Jump");
    }

    private void OnClimb()
    {
        Debug.Log("Climb");
    }

    private void OnAnchorInteract()
    {
        Debug.Log("Anchor interact");
    }

    private void OnTug()
    {
        Debug.Log("Tug");
    }

    private void OnWorldInteract()
    {
        Debug.Log("World interact");
    }

    private void OnAim(InputValue value)
    {
        if (!m_AnchorInteract.IsPressed())
        {
            return;
        }

        var direction = value.Get<Vector2>();

        Debug.Log($"Aiming ({Vector2.SignedAngle(Vector2.right, direction)} degrees)");
    }
}
