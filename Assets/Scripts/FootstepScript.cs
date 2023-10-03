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
        AudioController.PlaySound(m_FootstepSounds[0], 0.3f, Random.Range(0.95f,1.05f), MixerGroup.SFX);
        Debug.Log("Left footstep");
        Footstep.Invoke();
    }

    public void RightFootstep()
    {
        AudioController.PlaySound(m_FootstepSounds[1], 0.3f, Random.Range(0.95f, 1.05f), MixerGroup.SFX);

        Debug.Log("Right footstep");
        Footstep.Invoke();
    }
}
