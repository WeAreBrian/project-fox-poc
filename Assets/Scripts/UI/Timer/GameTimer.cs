using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_TimerTextMesh;

    private string m_TimerText;
    private float m_TimeElapsed;
    private bool m_TimerIsRunning;

    private void Start()
    {
        ResetTimer();
        StartTimer();
    }

    private void Update()
    {
        if (m_TimerIsRunning)
        {
            m_TimeElapsed += Time.deltaTime;
            m_TimerText = m_TimeElapsed.ToString("00:00.00");
            m_TimerTextMesh.text = m_TimerText;
        }
    }

    public void ResetTimer()
    {
        m_TimerText = "00:00.00";
        m_TimeElapsed = 0f;
        m_TimerIsRunning = false;
    }

    // Start timer, can also be used for resuming timer
    public void StartTimer()
    {
        m_TimerIsRunning = true;
    }

    // Pause timer, can be assigned to the GameEnd event (when it's implemented), and game pause
    // NOTE: Scoreboard related implementation should not be here (comment to be deleted after scoreboard implementation)
    public void PauseTimer()
    {
        m_TimerIsRunning = false;
    }
}