using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class ControlsManager : MonoBehaviour
{
    private bool m_DevMode;

    private InputAction m_VolumeControl;
    private InputAction m_SelectMusic;
    private InputAction m_SelectSFX;
    private InputAction m_TimeoutScreen;
    private InputAction m_Scoreboard;
    private InputAction m_MuteControl;

    private AudioMixer m_Mixer;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (SceneManager.GetActiveScene().name == "Preload") SceneManager.LoadScene(1);

        PlayerInput input = FindAnyObjectByType<PlayerInput>();

        m_VolumeControl = input.actions["VolumeControl"];
        m_MuteControl = input.actions["MuteControl"];
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
        if (m_SelectMusic.ReadValue<float>() > 0.5f)
        {
            UpdateVolume("MusicVolume");
        }
        else if (m_SelectSFX.triggered)
        {
            UpdateVolume("SFXVolume");
        }
    }

    private void UpdateVolume(string parameterName)
    {

        float volume;
        m_Mixer.GetFloat(parameterName, out volume);
        m_Mixer.SetFloat(parameterName, Mathf.Clamp(volume, -80, 20));
    }
}
