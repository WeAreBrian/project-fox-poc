 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.CullingGroup;

public class Anchor : MonoBehaviour
{
    public UnityEvent<AnchorState> StateChanged;

    public Rigidbody2D Rigidbody => m_Rigidbody;
    public AnchorState State => m_State;

    private AnchorState m_State;
    private Rigidbody2D m_Rigidbody;
    private Timer m_FreeTimer;

    private bool m_Shake;
    [SerializeField]
    private float m_ShakeFrequency;
    [SerializeField]
    private AnimationCurve m_ShakeAmplitude;
    private float m_ShakeDuration;
    private float m_ShakeAmplitudeTimer;
    private Vector3 m_ShakePos;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

        m_Rigidbody.useFullKinematicContacts = true;

        m_FreeTimer = new Timer();
    }

    private void Update()
    {
        m_FreeTimer.Tick();

        if (m_Shake)
        {
            m_ShakeAmplitudeTimer -= Time.deltaTime;
            float x = transform.position.x * Mathf.Sin(Time.time * m_ShakeFrequency * m_ShakeAmplitude.Evaluate(1 - (m_ShakeAmplitudeTimer / m_ShakeDuration))) * 0.005f;
            float y = transform.position.y * Mathf.Sin(Time.time * m_ShakeFrequency* m_ShakeAmplitude.Evaluate(1 - (m_ShakeAmplitudeTimer / m_ShakeDuration))) * 0.005f;
            float z = transform.position.z;

            // Then assign a new vector3
            gameObject.transform.position = m_ShakePos + new Vector3(x, y, z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grapplable"))
        {
            UpdateState(AnchorState.Lodged);
        }
        else
        {
            foreach (ContactPoint2D hitpos in collision.contacts)
            {
                var objectBounds = collision.gameObject.GetComponent<Collider2D>().bounds;
                if (hitpos.point.x < objectBounds.min.x+0.1f || hitpos.point.x > objectBounds.max.x-0.1f || hitpos.point.y < objectBounds.min.y+0.1f)
                {
                    Debug.Log("hit a side");
                    UpdateState(AnchorState.Free);
                    return;
                }
            }
            UpdateState(AnchorState.Grounded);
        }
    }

    public void ActivateShake(float duration)
    {
        StartCoroutine(Shake(duration));
    }

    private IEnumerator Shake(float duration)
    {
        m_Shake = true;
        m_ShakePos = transform.position;
        m_ShakeAmplitudeTimer = duration;
        m_ShakeDuration = duration;
        yield return new WaitForSeconds(duration);
        m_Shake = false;
        transform.position = m_ShakePos;
    }


    public void PickUp()
    {
        UpdateState(AnchorState.Held);
    }

    private void UpdateState(AnchorState next)
    {
        Debug.Log("Setting state to " + next);

        if (!m_FreeTimer.Paused)
        {
            m_State = AnchorState.Free;
            m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            return;
        }

        //Reset rotation when anchor is picked up
        if (next == AnchorState.Held)
        {
            m_Rigidbody.rotation = 0;
        }

        // Set body type based on state
        if (next == AnchorState.Free || next == AnchorState.Held)
        {
            m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            //m_Rigidbody.bodyType = RigidbodyType2D.Static;

            m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = 0;
        }

        StateChanged?.Invoke(next);

		m_State = next;
    }

    public void Drop()
    {
        UpdateState(AnchorState.Free);
    }

    public void Throw(Vector2 velocity)
    {
        m_Rigidbody.velocity = velocity;
    }

    public void Dislodge(Vector2 velocity)
    {
        UpdateState(AnchorState.Free);

        m_Rigidbody.velocity = velocity;
	}

    public void FreeForDuration(float seconds)
    {
        m_FreeTimer.Start(seconds);
        UpdateState(AnchorState.Free);
    }
}
