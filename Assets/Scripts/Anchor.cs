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

	[SerializeField]
	private GameObject m_AnchorImpactImage;
	private GameObject m_SpawnedAnchorImpactImage;

	private Collision2D m_Collision;

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

		if (m_Shake)
		{
			var progress = (m_ShakeAmplitudeTimer / m_ShakeDuration);

			m_ShakeAmplitudeTimer -= Time.deltaTime;
			float x = transform.position.x * Mathf.Sin(Time.time * m_ShakeFrequency * m_ShakeAmplitude.Evaluate(1-progress)) * 0.01f* m_ShakeAmplitude.Evaluate(1 - progress);
			float y = transform.position.y * Mathf.Sin(Time.time * m_ShakeFrequency*1.2f* m_ShakeAmplitude.Evaluate(1-progress)) * 0.01f* m_ShakeAmplitude.Evaluate(1 - progress);
			float z = 0;

			m_TimerSprite.transform.localScale = new Vector3(1.5f * progress, 1.5f * progress, 1.5f * progress);

			// Then assign a new vector3
			gameObject.transform.position = m_ShakePos + new Vector3(x, y, z);
		}
	}
	
	private void OnCollisionEnter2D(Collision2D collision)
	{
		m_Collision = collision;    //For spawning anchor impact image
		if (collision.gameObject.CompareTag("Grapplable"))
		{
			UpdateState(AnchorState.Lodged);
		}
		else if (!collision.gameObject.CompareTag("Player"))
		{
			foreach (ContactPoint2D hitpos in collision.contacts)
			{
				if (hitpos.normal != Vector2.up)
				{
					Debug.Log("hit a side");
					AudioController.PlaySound(m_AnchorBump, 1, 1, MixerGroup.SFX);
					CameraShake.instance.Shake(1, 0.2f);
					HapticManager.instance.RumblePulse(0.25f, 0.25f, 0.1f);
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
		m_TimerSprite.SetActive(true);
		yield return new WaitForSeconds(duration);
		m_Shake = false;
		transform.position = m_ShakePos;
		m_TimerSprite.SetActive(false);
	}

	private IEnumerator DisableFoxCollision()
	{
		gameObject.layer = LayerMask.NameToLayer("Ghost");
		yield return new WaitForSeconds(0.1f);
		gameObject.layer = LayerMask.NameToLayer("Anchor");
		var collider = GetComponent<Collider2D>();
		collider.enabled = true;

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
			HapticManager.instance.RumblePulse(0.5f, 0.5f, 0.05f);
			m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
		}
		else
		{
			//m_Rigidbody.bodyType = RigidbodyType2D.Static;

			m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
			m_Rigidbody.velocity = Vector3.zero;
			m_Rigidbody.angularVelocity = 0;

			if (next == AnchorState.Lodged)
			{
				SpawnAnchorImpactImage();
				HapticManager.instance.RumblePulse(0.5f, 0.75f, 0.05f);
				CameraShake.instance.Shake(2, 0.2f);
				AudioController.PlaySound(m_AnchorLodge, 1, 1, MixerGroup.SFX);
			}
			else
			{
				SpawnAnchorImpactImage();
				HapticManager.instance.RumblePulse(0.25f, 1f, 0.1f);
				CameraShake.instance.Shake(2, 0.1f);
				AudioController.PlaySound(m_AnchorLand, 1, 1, MixerGroup.SFX);
			}
		}

		StateChanged?.Invoke(next);

		m_State = next;
	}

	private void SpawnAnchorImpactImage()
	{
        //Destroy(m_SpawnedAnchorImpactImage);

        // Get position and normal of the collision where anchor hit object
        Vector3 m_CollisionPosition = m_Collision.contacts[0].point;
        Vector2 m_CollisionNormal = m_Collision.contacts[0].normal;

        // Define a layer mask to target the "Terrain" layer
        LayerMask m_TerrainLayerMask = LayerMask.GetMask("Terrain");

        // Calculate ray direction using collision normal
        Vector2 m_RayDirection = -m_CollisionNormal;

        // Calculate spawn position using a raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, m_RayDirection, Mathf.Infinity, m_TerrainLayerMask);
        Vector3 m_SpawnPosition = hit.point;

        // Get an angle out of the normal vector
        float m_Angle = Mathf.Atan2(m_CollisionNormal.y, m_CollisionNormal.x) * Mathf.Rad2Deg;
        m_Angle -= 90; // This is to properly orient the sprite

        // Turn angle into rotation
        Quaternion m_Rotation = Quaternion.AngleAxis(m_Angle, Vector3.forward);

        m_SpawnedAnchorImpactImage = Instantiate(m_AnchorImpactImage, m_SpawnPosition, m_Rotation);

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
