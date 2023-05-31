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
    [SerializeField]
    private GameObject m_TimerSprite;

    [SerializeField]
    private AudioClip m_AnchorLand;
    [SerializeField]
    private AudioClip m_AnchorLodge;
    [SerializeField]
    private AudioClip m_AnchorBump;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

        m_Rigidbody.useFullKinematicContacts = true;

        m_FreeTimer = new Timer();

        m_TimerSprite.SetActive(false);
    }

    private void Update()
    {
        m_FreeTimer.Tick();
        if (m_Shake) Shake();
    }

    private void Shake()
    {
        var progress = (m_ShakeAmplitudeTimer / m_ShakeDuration);

        m_ShakeAmplitudeTimer -= Time.deltaTime;
        float x = transform.position.x * Mathf.Sin(Time.time * m_ShakeFrequency * m_ShakeAmplitude.Evaluate(1 - progress)) * 0.01f * m_ShakeAmplitude.Evaluate(1 - progress);
        float y = transform.position.y * Mathf.Sin(Time.time * m_ShakeFrequency * 1.2f * m_ShakeAmplitude.Evaluate(1 - progress)) * 0.01f * m_ShakeAmplitude.Evaluate(1 - progress);
        float z = 0;

        m_TimerSprite.transform.localScale = new Vector3(1.5f * progress, 1.5f * progress, 1.5f * progress);

        // Then assign a new vector3
        gameObject.transform.position = m_ShakePos + new Vector3(x, y, z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grapplable"))
        {
            UpdateState(AnchorState.Lodged);
        }
        else if (!collision.gameObject.CompareTag("Player"))
        {

            foreach (ContactPoint2D hitpos in collision.contacts)
            {
                if (IsTop(hitpos.point, collision.gameObject.GetComponent<Collider2D>().bounds))
                {
                    UpdateState(AnchorState.Free);
                }
            }
            UpdateState(AnchorState.Grounded);
        }
    }

    //This is just a wrapper method so that the coroutine can be started externally without calling coroutine in a different script
    //Coroutines fucking suck though and i wanna change this but and also we arent going to use stall probably so who fucking cares
    public void ActivateShake(float duration)
    {
        StartCoroutine(Shake(duration));
    }
    
    //This will return true if the collision point is above the bounds of the objectBounds
    //This will only be accurate for objects which are axis oriented so this solution needs to be revisited (ie platforms on an angle will not work with this solution
    private bool IsTop(Vector2 collisionPoint, Bounds objectBounds)
    {
        if (collisionPoint.x < objectBounds.min.x + 0.1f || collisionPoint.x > objectBounds.max.x - 0.1f || collisionPoint.y < objectBounds.min.y + 0.1f)
        {
            return false;
        }
        return true;
    }

    //Increases shake amplitude over duration
    //Resets position after shake is finished so that it doesnt actually move when the shake is finished
    private IEnumerator Shake(float duration)
    {
        m_Shake = true;
        m_ShakePos = transform.position;
        m_ShakeAmplitudeTimer = duration;
        m_ShakeDuration = duration;
        m_TimerSprite.SetActive(true);
        yield return new WaitForSeconds(duration);
        m_Shake = false;
        transform.position = m_ShakePos;
        m_TimerSprite.SetActive(false);
    }


    //Moves Object to ghost layer so that it will not interact with the player for the duration specified
    //This will prevent the anchor from hitting the player when the anchor is thrown
    private IEnumerator DisableFoxCollision()
    {
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        yield return new WaitForSeconds(0.1f);
        gameObject.layer = LayerMask.NameToLayer("Anchor");

    }


    public void PickUp()
    {
        UpdateState(AnchorState.Held);
    }

    //Updates the rigidbody and whatnot based on the state that the anchor is entering
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

        //play sound based on state
        if (next == AnchorState.Lodged)
        {
            AudioController.PlaySound(m_AnchorLodge, 1, 1, MixerGroup.SFX);
        }
        else if (next == AnchorState.Free)
        {

            AudioController.PlaySound(m_AnchorBump, 1, 1, MixerGroup.SFX);
        }
        else
        {
            AudioController.PlaySound(m_AnchorLand, 1, 1, MixerGroup.SFX);
        }

        StateChanged?.Invoke(next);

		m_State = next;
    }

    public void Drop()
    {
        UpdateState(AnchorState.Free);
        StartCoroutine(DisableFoxCollision());
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
