using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;



public class LevelEndUI : MonoBehaviour
{
    private float m_LastScore;
    private TextMeshProUGUI m_ScoreText;

    private CloseOrOpenCircle m_HoleTransition;


    // Start is called before the first frame update
    private void Start()
    {
        m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
        m_LastScore = PlayerPrefs.GetFloat("LastScore", 999999);
        m_ScoreText = GetComponent<TextMeshProUGUI>();
        m_ScoreText.text = FormatTime(m_LastScore);
    }


    public void RestartLevelButtonPressed()
    {
        string m_LastScene = PlayerPrefs.GetString("LastScene");
        StartCoroutine(m_HoleTransition.ShrinkParentObject(m_LastScene));
    }

    public void LevelSelectButtonPressed()
    {
        SceneManager.LoadScene("LevelSelection");
        //StartCoroutine(m_HoleTransition.ShrinkParentObject("LevelSelection"));
    }

    private string FormatTime(float m_TimeInSeconds)
    {
        // Extract the minutes portion of the time
        int m_Minutes = (int)(m_TimeInSeconds / 60);

        // Extract the remaining seconds (excluding the minutes)
        int m_Seconds = (int)(m_TimeInSeconds % 60);

        // Extract the milliseconds (excluding the seconds and minutes)
        int m_Milliseconds = (int)((m_TimeInSeconds * 1000) % 1000);

        // Create a formatted string
        return string.Format("{0:00}:{1:00}:{2:000}", m_Minutes, m_Seconds, m_Milliseconds);
    }

}
