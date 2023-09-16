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
        m_ScoreText.text = TimeFormatter.Milliseconds(m_LastScore);
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
}
