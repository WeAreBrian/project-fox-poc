using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FadeAndDestroy : MonoBehaviour
{
    SpriteRenderer[] m_SpriteRenderers;

    [SerializeField]
    private float m_FadeOutDelay;
    [SerializeField]
    private float m_FadeOutLength;
    [SerializeField]
    private float m_DestroyTimer = 5f;
    [SerializeField]
    private bool m_ShouldShrink = true;


    private float m_Timer;
    private Vector2 m_OriginalScale;

    private void Awake()
    {
        m_SpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        m_FadeOutDelay = 0; //Random.Range(0.5f, 1.5f);
        m_FadeOutLength = 0.2f; //Random.Range(0.2f, 0.5f);

        m_OriginalScale = transform.localScale;
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

            if (m_ShouldShrink)
            {
                // new scale towards 0
                Vector2 m_NewScale = Vector2.Lerp(m_OriginalScale, m_OriginalScale * 0.9f, progress);
                transform.localScale = m_NewScale;
            }
            

        }

        if (m_Timer > m_DestroyTimer)
        {
            Destroy(gameObject);
        }
    }
}
