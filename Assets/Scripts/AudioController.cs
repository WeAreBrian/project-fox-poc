using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public enum MixerGroup
{
    Master,
    SFX,
    Dialogue,
    Music
}

public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    [SerializeField]
    private AudioSource music;

    private AudioSource sound;

    private static AudioMixer mixer;
    [SerializeField]
    private AudioMixer inspectorMixer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            mixer = inspectorMixer;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        try
        {
            sound = GameObject.Find("Sound").GetComponent<AudioSource>();
            if (!sound.isPlaying)
            {
                Destroy(sound.gameObject);
            }
        }
        catch
        {

        }
    }

    public static GameObject PlaySound(AudioClip clip, float volume, float pitch, MixerGroup mixerGroup)
    {
        if (Time.timeScale == 0)
        {
            return null;
        }

        GameObject soundGameObject = new GameObject("Sound");
        DontDestroyOnLoad(soundGameObject);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = GetMixerGroup(mixerGroup);
        audioSource.volume = volume;
        audioSource.bypassEffects = false;
        audioSource.pitch = pitch;

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }

        return audioSource.gameObject;
    }

    private static AudioMixerGroup GetMixerGroup(MixerGroup group)
    {
        return mixer.FindMatchingGroups(group.ToString())[0];
    }
}
