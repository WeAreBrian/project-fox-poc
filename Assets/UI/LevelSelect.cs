using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    private string m_Level1;
    [SerializeField]
    private string m_Level2;
    [SerializeField]
    private string m_Level3;
    private VisualElement root;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        //Get buttons
        Button m_Level1Button = root.Q<Button>("Level1");
        Button m_Level2Button = root.Q<Button>("Level2");
        Button m_Level3Button = root.Q<Button>("Level3");

        //Assign event for the buttons
        m_Level1Button.clicked += () => SceneManager.LoadScene(m_Level1);
        m_Level1Button.clicked += () => SceneManager.LoadScene(m_Level2);
        m_Level1Button.clicked += () => SceneManager.LoadScene(m_Level3);

    }
}
