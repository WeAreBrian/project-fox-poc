using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPrefabSpawner : MonoBehaviour
{
    public void SpawnAnimationPrefab(GameObject m_AnimationPrefab, float m_AnimationSpeed, Vector3 m_Position = new Vector3())
    {
        // Instantiate the prefab
        GameObject m_AnimationInstance = Instantiate(m_AnimationPrefab, transform.position + m_Position, Quaternion.identity);

        // Get the Animator component from the spawned object
        Animator m_Animator = m_AnimationInstance.GetComponent<Animator>();

        if (m_Animator != null)
        {
            // Get the speed and duration of the animation
            AnimatorClipInfo[] m_ClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0); // Assuming the animation is in layer 0
            AnimationClip m_AnimationClip = m_ClipInfo[0].clip;

            m_Animator.speed = m_AnimationSpeed;
            float m_AdjustedAnimationDuration = m_AnimationClip.length / m_AnimationSpeed;

            // Destroy the object after the adjusted animation duration
            Destroy(m_AnimationInstance, m_AdjustedAnimationDuration);
        }
    }
}
