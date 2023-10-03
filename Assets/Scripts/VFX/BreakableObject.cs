using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject m_WoodExplodeVFX;

    [SerializeField]
    private GameObject m_WoodVFX;

    [SerializeField]
    private AudioClip m_ImpactSound;

    [SerializeField]
    private List<AudioClip> m_ClatterSounds;

    [SerializeField]
    private GameObject m_Plank;

    private Collider2D m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
    }

    //Determine impact direction using anchor's velocity at moment of impact
    private Quaternion DetermineImpactDirection(Vector3 rbVelocity)
    {
        // If the wall is rotated, also rotate velocity vector to compensate
        float objectRotation = transform.localEulerAngles.z;
        Quaternion invertedRotation = Quaternion.Euler(0f, 0f, -objectRotation);
        Vector2 localRbVelocity = invertedRotation * rbVelocity;

        if (localRbVelocity.x > 0)
        {
            // If velocity is locally positive, the wall is hit from the back
            // The FX is facing forward by default
            return Quaternion.Euler(0, 0, 0);
        }

        // If velocity is locally negative, the wall is hit from the front
        // We'll flip the FX accordingly
        return Quaternion.Euler(0, 180, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Anchor"))
        {
            Rigidbody2D AnchorRb = collision.GetComponent<Rigidbody2D>();
            m_WoodVFX.transform.localRotation = DetermineImpactDirection(AnchorRb.velocity);

            // Disable scripts and objects (avoid destroy to avoid having to clean up)
            m_Collider.enabled = false;
            m_Plank.SetActive(false);

			HapticManager.instance.RumblePulse(0.25f, 1f, 0.1f);
            CameraShake.instance.Shake(2, 0.2f);
            AudioController.PlaySound(m_ImpactSound, 0.5f, 1, MixerGroup.SFX);
            PlayClatterSounds(Random.Range(2,5));
            m_WoodExplodeVFX.SetActive(true);
        }
    }

    private void PlayClatterSounds(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AudioController.PlaySound(m_ClatterSounds[Random.Range(0, m_ClatterSounds.Count)], 0.4f, Random.Range(0.95f, 1.05f), MixerGroup.SFX);
        }
    }
}
