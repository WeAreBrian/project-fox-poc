using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InactivityChecker : MonoBehaviour
{
    private float m_lastTime;
    [SerializeField]
    private float m_secondsUntilReset;
    [SerializeField]
    private int m_sceneIndexToLoad;

    // Start is called before the first frame update
    void Start()
    {
        m_lastTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //detect any key and reset timer
        if (Input.anyKey)
        {
            m_lastTime = Time.fixedTime;
        }

        //if timer is past x seconds, reset
        if(Time.fixedTime - m_lastTime > m_secondsUntilReset)
        {
            ResetGame();
        }
    }

    void ResetGame()
    {
        SaveUtils.InitializeProfile();
        SceneManager.LoadScene(m_sceneIndexToLoad);
	}
}
