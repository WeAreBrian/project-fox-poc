using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class LevelSelect : MonoBehaviour
{
    VisualElement root;
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        
        Button m_PlayButton = root.Q<Button>("Play");
        m_PlayButton.clicked += () => SceneManager.LoadScene("A");
        
    }
}
