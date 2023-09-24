using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSounds : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> m_ChainStepSounds;
    [SerializeField]
    private AudioClip m_PullTautSound;
    [SerializeField]
    private AudioClip m_GrappleTautSound;
    [SerializeField]
    private List<AudioClip> m_ArialChainSounds;

    private AudioClip RandomChainStepSound => m_ChainStepSounds[Random.Range(0, m_ChainStepSounds.Count)];
    private AudioClip RandomChainArialSound => m_ArialChainSounds[Random.Range(0, m_ArialChainSounds.Count)];

    private Grounded m_Grounded;
    private PhysicsChain m_PhysicsChain;

    private Timer arialSoundCooldown;

    private void Start()
    {
        m_Grounded = FindObjectOfType<Grounded>();
        m_PhysicsChain = FindObjectOfType<PhysicsChain>();

        arialSoundCooldown = new Timer();
        arialSoundCooldown.Start();
        arialSoundCooldown.Looping = true;

        FindObjectOfType<FootstepScript>().Footstep.AddListener(ChainSound);
        GetComponent<IdealChain>().PulledTaut.AddListener(OnPullTaut);
        m_Grounded.LeftGround.AddListener(ArialSound);
        arialSoundCooldown.Completed += ArialSound;
    }

    private void Update()
    {

        arialSoundCooldown.Tick();
    }

    private void ArialSound()
    {
        var coefficient = Mathf.Clamp(1 / m_PhysicsChain.AveragedChainSpeed, 0.1f, 1);
        arialSoundCooldown.Duration = coefficient;

        if (!m_Grounded.OnGround)
        {
            AudioController.PlaySound(RandomChainArialSound, coefficient * 0.5f, 1, MixerGroup.SFX);
        }
        
    }

    private void ChainSound()
    {
        AudioController.PlaySound(RandomChainStepSound, 0.5f, 1, MixerGroup.SFX);
    }

    private void OnPullTaut(float force)
    {
        //this scales the sound approximately according to walking speed
        force = force / 12;
        AudioController.PlaySound(m_PullTautSound, 0.1f*force, Random.Range(0.95f,1.05f), MixerGroup.SFX);
    }

    private void GrappleTaut()
    {
        AudioController.PlaySound(m_GrappleTautSound, 0.5f, 1, MixerGroup.SFX);
    }
}
