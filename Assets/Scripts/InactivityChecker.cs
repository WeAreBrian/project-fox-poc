using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InactivityChecker : MonoBehaviour
{
    private float m_CurrentResetTime;
    private float m_CurrentInactiveTime;

    [SerializeField]
    private float m_SecondsUntilReset;
    [SerializeField]
    private float m_InactiveTimeLimit = 60f;
    [SerializeField]
    private int m_SceneIndexToLoad;
    [SerializeField]
    private TextMeshProUGUI m_Text;
    [SerializeField]
    private GameObject m_InactivityTimerContainer;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentResetTime = m_SecondsUntilReset;
        m_CurrentInactiveTime = m_InactiveTimeLimit;
        m_InactivityTimerContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrentInactiveTime -= Time.deltaTime;

        if (m_CurrentInactiveTime <= m_SecondsUntilReset)
        {
            m_InactivityTimerContainer.SetActive(true);
            m_CurrentResetTime -= Time.deltaTime;
            m_Text.text = Mathf.Round(m_CurrentResetTime).ToString();

            //if timer is past x seconds, reset
            if (m_CurrentInactiveTime <= 0f)
            {
                ResetGame();
            }
        }

        //if preses any key, reset timer
        if (Input.anyKey)
        {
            RestTimer();
        }
    }

    private void ResetGame()
    {
        SaveUtils.InitializeProfile();
        SceneManager.LoadScene(m_SceneIndexToLoad);
    }

    private void RestTimer()
    {
        m_CurrentInactiveTime = m_InactiveTimeLimit;
        m_CurrentResetTime = m_SecondsUntilReset;
        m_InactivityTimerContainer.SetActive(false);
    }
}
