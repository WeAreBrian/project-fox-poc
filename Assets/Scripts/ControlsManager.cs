using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    private bool m_DevMode;

    private InputAction m_VolumeControl;
    private InputAction m_SelectMusic;
    private InputAction m_SelectSFX;
    private InputAction m_TimeoutScreen;
    private InputAction m_Scoreboard;

    private 

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (SceneManager.GetActiveScene().name == "Preload") SceneManager.LoadScene(1);

        PlayerInput input = FindAnyObjectByType<PlayerInput>();

        m_VolumeControl = input.actions["VolumeControl"];
        m_SelectMusic = input.actions["SelectMusic"];
        m_SelectSFX = input.actions["SelectSFX"];
        m_TimeoutScreen = input.actions["TimeoutScreen"];
        m_Scoreboard = input.actions["Scoreboard"];

    }

    private void Update()
    {
        var volumeModifier = m_VolumeControl.ReadValue<float>();
        if (volumeModifier != 0)
        {
            
        }
    }
}
