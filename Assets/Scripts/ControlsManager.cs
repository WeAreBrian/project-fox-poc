using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class ControlsManager : MonoBehaviour
{
    private bool m_DevMode;

    private PlayerInput m_Input;

    private InputAction m_VolumeControl;
    private InputAction m_SelectMusic;
    private InputAction m_SelectSFX;
    private InputAction m_TimeoutScreen;
    private InputAction m_Scoreboard;
    private InputAction m_MuteControl;

    [SerializeField]
    private AudioMixer m_Mixer;

    private float m_VolumeBeforeMute;

    private bool doubleTapReady;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (SceneManager.GetActiveScene().name == "Preload") SceneManager.LoadScene(1);



        SceneManager.sceneLoaded += UpdateInput;
        UpdateInput(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void UpdateInput(Scene scene, LoadSceneMode mode)
    {
        m_Input = FindAnyObjectByType<PlayerInput>();

        Debug.Log("PlayerInput" + m_Input);

        if (m_Input == null) return;

        m_VolumeControl = m_Input.actions["VolumeControl"];
        m_MuteControl = m_Input.actions["MuteControl"];
        m_SelectMusic = m_Input.actions["SelectMusic"];
        m_SelectSFX = m_Input.actions["SelectSFX"];
        m_TimeoutScreen = m_Input.actions["TimeoutScreen"];
        m_Scoreboard = m_Input.actions["Scoreboard"];
    }

    private void Update()
    {
        if (m_Input == null) return; 

        var volumeModifier = m_VolumeControl.ReadValue<float>();
        if (m_VolumeControl.triggered)
        {
            if (m_SelectMusic.ReadValue<float>() > 0.5f)
            {
                UpdateVolume("MusicVolume", volumeModifier * 5);
            }
            if (m_SelectSFX.ReadValue<float>() > 0.5f)
            {
                UpdateVolume("SFXVolume", volumeModifier * 5);
            }
        }
        if (m_MuteControl.triggered)
        {
            float volume;
            m_Mixer.GetFloat("MusicVolume", out volume);
            m_Mixer.SetFloat("MusicVolume", m_VolumeBeforeMute > volume ? m_VolumeBeforeMute : -80);
            m_VolumeBeforeMute = volume;
        }
        if (m_TimeoutScreen.triggered)
        {
            if (!doubleTapReady)
            {
                StartCoroutine(WaitForDoubleTap());
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }

    private IEnumerator WaitForDoubleTap()
    {
        doubleTapReady = true;
        yield return new WaitForSeconds(1);
        doubleTapReady = false;
    }

    private void UpdateVolume(string parameterName, float increment)
    {
        float volume;
        m_Mixer.GetFloat(parameterName, out volume);
        m_Mixer.SetFloat(parameterName, Mathf.Clamp(volume+increment, -80, 20));
    }
}
