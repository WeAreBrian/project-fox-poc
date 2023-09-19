using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepScript : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> m_FootstepSounds;

    public void LeftFootstep()
    {
        AudioController.PlaySound(m_FootstepSounds[0], 0.3f, 1, MixerGroup.SFX);
    }

    public void RightFootstep()
    {
        AudioController.PlaySound(m_FootstepSounds[1], 0.3f, 1, MixerGroup.SFX);
    }
}
