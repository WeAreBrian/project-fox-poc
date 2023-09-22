using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FootstepScript : MonoBehaviour
{
    public UnityEvent Footstep;

    [SerializeField]
    private List<AudioClip> m_FootstepSounds;

    public void LeftFootstep()
    {
        AudioController.PlaySound(m_FootstepSounds[0], 0.3f, 1, MixerGroup.SFX);
        Footstep.Invoke();
    }

    public void RightFootstep()
    {
        AudioController.PlaySound(m_FootstepSounds[1], 0.3f, 1, MixerGroup.SFX);
        Footstep.Invoke();
    }
}
