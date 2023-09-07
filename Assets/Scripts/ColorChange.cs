using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;
    [SerializeField]
    private Color m_SubmergedColor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            m_SpriteRenderer.color = m_SubmergedColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            m_SpriteRenderer.color = Color.white;
        }
    }
}
