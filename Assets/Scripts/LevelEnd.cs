using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField]
    private float m_EndDelay = 3f;

    private bool m_EndDelayStarted = false;

    private GrowAndShrink m_GrowAndShrink;

    private void Awake()
    {
        m_GrowAndShrink = transform.Find("CircleLight").GetComponent<GrowAndShrink>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Call the TimerAction method after the specified delay
        Invoke("TimerEnded", m_EndDelay);
        Destroy(m_GrowAndShrink);
    }

    private void TimerEnded()
    {

        if (!m_EndDelayStarted)
        {
            m_EndDelayStarted = true;
            // This method will be called after the specified delay
            Debug.Log("Timer action executed!");
        }
        
    }

    private void Update()
    {
        if (m_EndDelayStarted)
        {

        }
    }
}
