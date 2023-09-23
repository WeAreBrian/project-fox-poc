using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class LevelSelect : MonoBehaviour
{
    private string m_Level0 = "Onboarding";
    private string m_Level1 = "Brians Level Idea";
    private string m_Level2 = "Another Level From Brian For You You Are Welcome";
    private string m_Level3 = "More Levels from brian";
    private VisualElement m_LevelSelectRoot;
    private VisualElement m_PauseRoot;

    private void OnEnable()
    {
        m_LevelSelectRoot = GetComponent<UIDocument>().rootVisualElement;
        GetComponent<UIDocument>().enabled = true;
        m_LevelSelectRoot.style.display = DisplayStyle.None;

        GameObject m_PauseObject = GameObject.Find("PauseMenu");
		m_PauseRoot = m_PauseObject.GetComponent<UIDocument>().rootVisualElement;

        //Get buttons
        Button m_Level0Button = m_LevelSelectRoot.Q<Button>("Level0");
        Button m_Level1Button = m_LevelSelectRoot.Q<Button>("Level1");
        Button m_Level2Button = m_LevelSelectRoot.Q<Button>("Level2");
        Button m_Level3Button = m_LevelSelectRoot.Q<Button>("Level3");
        Button m_BackButton = m_LevelSelectRoot.Q<Button>("BackButton");

        //Assign event for the buttons
        m_Level0Button.clicked += () => LoadScene(m_Level0);
        m_Level1Button.clicked += () => LoadScene(m_Level1);
        m_Level2Button.clicked += () => LoadScene(m_Level2);
        m_Level3Button.clicked += () => LoadScene(m_Level3);
        m_BackButton.clicked += () => ReturnToPause();
    }

    private void ReturnToPause()
    {
        m_LevelSelectRoot.style.display = DisplayStyle.None;
        m_PauseRoot.style.display = DisplayStyle.Flex;

        var Button = m_PauseRoot.Q<Button>("Resume");
        Button.Focus();
    }
    private void LoadScene(string scene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
        AudioListener.pause = false;
    }
}
