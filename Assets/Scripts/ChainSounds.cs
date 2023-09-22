using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSounds : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> m_ChainStepSounds;

    private AudioClip RandomChainSound => m_ChainStepSounds[Random.Range(0, m_ChainStepSounds.Count)];

    private void Start()
    {
        FindObjectOfType<FootstepScript>().Footstep.AddListener(ChainSound);
    }

    private void ChainSound()
    {
        AudioController.PlaySound(RandomChainSound, 0.5f, 1, MixerGroup.SFX);
    }
}
