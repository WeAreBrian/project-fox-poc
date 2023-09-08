using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    [SerializeField]
    private float m_EndDelay = 3f;

    private bool m_EndDelayStarted = false;

    private CloseOrOpenCircle m_HoleTransition;
    private GameTimer m_GameTimer;

    private void Awake()
    {
        m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
        m_GameTimer = GameObject.Find("Speedrun Timer").GetComponent<GameTimer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Call the TimerAction method after the specified delay
        Invoke("TimerEnded", m_EndDelay);
        PlayerPrefs.SetFloat("LastScore", m_GameTimer.TimeElapsed); // This line will be deleted after FOX-219. This is currently preserved to keep the level end ui working
        SaveUtils.RecordTime(m_GameTimer.TimeElapsed);
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        // SaveUtils.SaveProfile(); // To be deleted after FOX-232
    }

    private void TimerEnded()
    {
        if (!m_EndDelayStarted)
        {
            m_EndDelayStarted = true;
            // This method will be called after the specified delay

            //Debug.Log("Timer action executed!");
            StartCoroutine(m_HoleTransition.ShrinkParentObject("LevelEnd"));

        }
    }
}
