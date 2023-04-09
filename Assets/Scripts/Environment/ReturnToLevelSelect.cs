using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ReturnToLevelSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject m_returnButton;

    [SerializeField]
    private GameObject m_eventSystem;

    public static ReturnToLevelSelect m_instance;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (m_instance == this)
        {
            if (scene.buildIndex != 0)
                m_returnButton.SetActive(true);

            // This check will be redundant later when every level has UI and a respective EventSystem
            if (FindObjectOfType<EventSystem>() == null)
                m_eventSystem.SetActive(true);
        }
    }

    public void ResetButton() {
        m_returnButton.SetActive(false);
        m_eventSystem.SetActive(false);
    }
}
