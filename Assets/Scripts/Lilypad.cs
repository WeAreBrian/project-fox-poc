using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lilypad : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    [SerializeField]
    private float m_RotationLimit;

    private float m_SpawnPositionX;

    private void Start()
    {
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            LeanTween.cancel(m_SpriteRenderer.gameObject);
            var relativePoint = transform.position.x - collision.contacts[0].point.x;
            var fractionOfSize = relativePoint / (collision.collider.bounds.size.x * 0.5f);
            m_SpriteRenderer.transform.rotation = Quaternion.Euler(0,0,m_RotationLimit * fractionOfSize);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        LeanTween.rotate(m_SpriteRenderer.gameObject, new Vector3(0,0,0), 1f).setDelay(0.5f);
    }

}
