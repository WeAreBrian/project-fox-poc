using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{
    SpriteRenderer[] m_SpriteRenderers;

    [SerializeField]
    private float m_FadeOutDelay = 1f;
    [SerializeField]
    private float m_FadeOutLength = 1f;

    private float m_Timer;

    private void Awake()
    {
        m_SpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (m_SpriteRenderers == null)
        {
            Debug.Log("No sprite renderer found! Is this script in the right place? - Contact Sach for help :)");
        }
        else
        {
            m_Timer = 0.0f;
            foreach (SpriteRenderer m_SpriteRenderer in m_SpriteRenderers)
            {
                //do something here on spawn?
            }
        }
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer > m_FadeOutDelay)
        {
            //the delay is subtracted so it's not starting with already half faded.
            float progress = (m_Timer - m_FadeOutDelay) / m_FadeOutLength;

            //new alpha towards 0
            float alpha = Mathf.Lerp(1.0f, 0.0f, progress);

            //fade each sprite renderer
            foreach (SpriteRenderer m_SpriteRenderer in m_SpriteRenderers)
            {
                m_SpriteRenderer.color = new Color(m_SpriteRenderer.color.r, m_SpriteRenderer.color.g, m_SpriteRenderer.color.b, alpha);

            }

            //Destroy after finishes fading out
            if (alpha <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
