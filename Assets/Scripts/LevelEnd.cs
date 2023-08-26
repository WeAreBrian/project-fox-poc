using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField]
    private float m_EndDelay = 3f;

    private bool m_EndDelayStarted = false;


    private void Awake()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Call the TimerAction method after the specified delay
        Invoke("TimerEnded", m_EndDelay);
    }

    private void TimerEnded()
    {

        if (!m_EndDelayStarted)
        {
            m_EndDelayStarted = true;
            // This method will be called after the specified delay

            //Debug.Log("Timer action executed!");
        }
        
    }

    private void Update()
    {
        if (m_EndDelayStarted)
        {

        }
    }
}
