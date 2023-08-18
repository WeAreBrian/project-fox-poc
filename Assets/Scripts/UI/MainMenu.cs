using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    VisualElement m_MainMenuRoot;
    private VisualElement m_LevelSelectRoot;

    private void OnEnable()
    {
        //Get levelselect object and root
        GameObject m_LevelSelectObject = GameObject.Find("LevelSelectMenu");
        m_LevelSelectRoot = m_LevelSelectObject.GetComponent<UIDocument>().rootVisualElement;
        //get main menu root
        m_MainMenuRoot = GetComponent<UIDocument>().rootVisualElement;

        //getting all the buttons
        Button m_LevelSelectButton = m_MainMenuRoot.Q<Button>("Play"); //this is the play button
        Button m_SettingsButton = m_MainMenuRoot.Q<Button>("Settings");

        //Setting what dem buttons do
		m_LevelSelectButton.clicked += () => m_LevelSelectRoot.style.display = DisplayStyle.Flex;
        m_SettingsButton.clicked += () => Debug.Log("TODO Settings");

    }
}
