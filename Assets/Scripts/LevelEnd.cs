using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    [SerializeField]
    private float m_EndDelay = 3f;
    private float m_SlowDownTime = 0.5f;
    private bool m_EndDelayStarted = false;

    private CloseOrOpenCircle m_HoleTransition;
    private GameTimer m_GameTimer;
    private bool m_levelEnded;
    private GrowAndShrinkLevelEndGlow m_GrowShrinkScript;

    private void Awake()
    {
        m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
        m_GameTimer = GameObject.Find("Speedrun Timer").GetComponent<GameTimer>();
        m_GrowShrinkScript = GetComponentInChildren<GrowAndShrinkLevelEndGlow>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // A bandaid fix for a bug that causes OnTriggerEnter2D to be called twice
            if (m_levelEnded)
                return;

            m_levelEnded = true;


            //Shrink the glowing circle
            //m_GrowShrinkScript.ShrinkToNothing(m_EndDelay);
            m_GrowShrinkScript.TriggerGrowOrShrinkEvent(m_EndDelay);


            // Call the TimerAction method after the specified delay
            StartCoroutine(EndDelayEnded());
            SaveUtils.RecordTime(m_GameTimer.TimeElapsed);
            PlayerPrefs.SetInt("LastScene", SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void TimerEnded()
    {
        if (!m_EndDelayStarted)
        {
            m_EndDelayStarted = true;

            

            //Debug.Log("Timer action executed!");
            StartCoroutine(m_HoleTransition.ShrinkParentObject(SceneManager.GetActiveScene().buildIndex + 1)); // Load the next scene (level 3 will load leaderboard next)

        }
    }

    //Realtime delay instead of timescaled
    private IEnumerator EndDelayEnded()
    {
        yield return new WaitForSecondsRealtime(m_EndDelay);
        TimerEnded();
    }

    private void LateUpdate()
    {
        //This is in update to prevent bullet time from changing the timescale
        if (m_levelEnded)
        {
            Time.timeScale = m_SlowDownTime;
        }
    }
}
