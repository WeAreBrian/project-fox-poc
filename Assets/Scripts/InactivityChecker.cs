using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactivityChecker : MonoBehaviour
{
    private float m_lastTime;
    [SerializeField]
    private float m_secondsUntilReset;

    // Start is called before the first frame update
    void Start()
    {
        m_lastTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            Debug.Log("key pressed");
            m_lastTime = Time.fixedTime;
        }

        if(Time.fixedTime - m_lastTime > m_secondsUntilReset)
        {
            Debug.Log("reset game");
        }
    }

    void ResetInactiveTimer()
    {

    }

    void ResetGame()
    {

    }
}
