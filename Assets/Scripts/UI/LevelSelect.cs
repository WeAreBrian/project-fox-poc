using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    private string m_Level1 = "Level 1 Brian";
    [SerializeField]
    private string m_Level2 = "Forest Demo";
    [SerializeField]
    private string m_Level3 = "Harley Level 1";
    private VisualElement m_LevelSelectRoot;

    private void OnEnable()
    {
        m_LevelSelectRoot = GetComponent<UIDocument>().rootVisualElement;
        GetComponent<UIDocument>().enabled = true;
        m_LevelSelectRoot.style.display = DisplayStyle.None;
        //Get buttons
        Button m_Level1Button = m_LevelSelectRoot.Q<Button>("Level1");
        Button m_Level2Button = m_LevelSelectRoot.Q<Button>("Level2");
        Button m_Level3Button = m_LevelSelectRoot.Q<Button>("Level3");
        Button m_BackButton = m_LevelSelectRoot.Q<Button>("BackButton");


        //Assign event for the buttons
        m_Level1Button.clicked += () => LoadScene(m_Level1);
        m_Level2Button.clicked += () => LoadScene(m_Level2);
        m_Level3Button.clicked += () => LoadScene(m_Level3);
        m_BackButton.clicked += () => m_LevelSelectRoot.style.display = DisplayStyle.None;
    }

    private void LoadScene(string scene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }
}
