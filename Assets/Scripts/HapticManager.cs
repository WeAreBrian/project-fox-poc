using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticManager : MonoBehaviour
{
    public static HapticManager instance;

    private Gamepad m_Pad;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void RumblePulse(float lowFrequency, float highFrequency, float duration)
    {
        m_Pad = Gamepad.current;

        if (m_Pad != null)
        {

            m_Pad.SetMotorSpeeds(lowFrequency, highFrequency);
            StartCoroutine(StopRumble(duration));
        }
    }

    private IEnumerator StopRumble(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_Pad.SetMotorSpeeds(0, 0);
    }
}
