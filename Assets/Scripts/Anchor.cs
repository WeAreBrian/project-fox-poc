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
	[SerializeField]
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
	private LayerMask m_GroundMask;

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

	public Vector2 m_LastVelocity;


    private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody2D>();

		m_Rigidbody.useFullKinematicContacts = true;

		m_FreeTimer = new Timer();
	}

	private void Update()
	{
		m_FreeTimer.Tick();
	}

    private void FixedUpdate()
    {
		//Get the flying velocity
		if(m_State == AnchorState.Free)
		{
            m_LastVelocity = m_Rigidbody.velocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
	{
		m_Collision = collision;    //For spawning anchor impact image
		if (collision.gameObject.CompareTag("Grapplable"))
		{
			UpdateState(AnchorState.Lodged);
		}
		else if (m_GroundMask == (m_GroundMask | (1 << collision.gameObject.layer)))
		{
			foreach (ContactPoint2D hitpos in collision.contacts)
			{
				if (hitpos.normal != Vector2.up)
				{
					//Debug.Log("hit a side");
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

	// Checks what surface the anchorimpact is hit with and enables the sprite for it
	private void CheckWhatSurfaceCollided(GameObject m_AnchorImpact)
	{
		//Get the transform of the sprites hierachy in the prefab
		Transform m_AnchorImpactSpriteTransform = m_AnchorImpact.transform;
		GameObject m_AnchorImpactSprite;
        GameObject m_AnchorImpactParticleSystem;

        if (m_Collision.gameObject.name.Contains("HoneyGrappleSurface"))
        {
			//Enable honey sprite
			m_AnchorImpactSprite = m_AnchorImpactSpriteTransform.Find("Sprites/HoneySprite").gameObject;
			m_AnchorImpactParticleSystem = m_AnchorImpactSpriteTransform.Find("HoneyDebris").gameObject;
        }
        else
		{
            //Enable rock sprite
            m_AnchorImpactSprite =  m_AnchorImpactSpriteTransform.Find("Sprites/RockSprite").gameObject;
            m_AnchorImpactParticleSystem = m_AnchorImpactSpriteTransform.Find("RockDebris").gameObject;

        }

        //Enable sprite then randomly flip it
        m_AnchorImpactSprite.SetActive(true);
        m_AnchorImpactParticleSystem.SetActive(true);
        m_AnchorImpactSprite.GetComponent<SpriteRenderer>().flipX = Random.Range(0, 2) == 1;
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

	private IEnumerator DisableFoxCollision()
	{
		if (m_State != AnchorState.Held)
		{
			var collider = GetComponent<Collider2D>();
			collider.enabled = true;
		}

		gameObject.layer = LayerMask.NameToLayer("Ghost");
		yield return new WaitForSeconds(0.1f);
        gameObject.layer = LayerMask.NameToLayer("Anchor");
		

	}


	public void PickUp()
	{
		UpdateState(AnchorState.Held);
	}

	private void UpdateState(AnchorState next)
	{
		//Debug.Log("Setting state to " + next);


		if (!m_FreeTimer.Paused)
		{
			m_State = AnchorState.Free;
			m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
			return;
		}

		//Reset rotation when anchor is picked up
		if (next == AnchorState.Held)
		{
			//destroy last crater
            FadeAndDestroyAnchorImpact();
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
		//destroy any leftover craters that got spawned (issue with anchor states in some situtaions, e.g. throwing anchor at spikes standing next to it, doesnt cause it to lodge/ground)
		FadeAndDestroyAnchorImpact();

        // Get position and normal of the collision where anchor hit object
        Vector2 m_CollisionNormal = m_Collision.contacts[0].normal;

        // Define a layer mask to target the "Terrain" layer
        LayerMask m_TerrainLayerMask = LayerMask.GetMask("Terrain");

        // Calculate ray direction using collision normal
        Vector2 m_RayDirection = -m_CollisionNormal;

        // Calculate spawn position using a raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, m_RayDirection, Mathf.Infinity, m_TerrainLayerMask);
        Vector3 m_SpawnPosition = hit.point;

		//move the z position behind the anchor
		m_SpawnPosition.z = -0.12f;

        // Get an angle out of the normal vector
        float m_Angle = Mathf.Atan2(m_CollisionNormal.y, m_CollisionNormal.x) * Mathf.Rad2Deg;
        m_Angle -= 90; // This is to properly orient the sprite

        // Turn angle into rotation
        Quaternion m_Rotation = Quaternion.AngleAxis(m_Angle, Vector3.forward);

        m_SpawnedAnchorImpactImage = Instantiate(m_AnchorImpactImage, m_SpawnPosition, m_Rotation);

		//Set the correct sprite based on which surface it hit
		CheckWhatSurfaceCollided(m_SpawnedAnchorImpactImage);

		//Get angle of the last velocity of anchor (while in the air)
        float m_ZRotationOfVelocity = Mathf.Atan2(m_LastVelocity.y, m_LastVelocity.x) * Mathf.Rad2Deg;
		//angle to rotation
        Quaternion m_RotationForDebris = Quaternion.Euler(new Vector3(0, 0, m_ZRotationOfVelocity - 90));


		//Set the debris rotation to be the angle the anchor came from
		m_SpawnedAnchorImpactImage.transform.Find("RockDebris/VelocityBasedDebris").transform.rotation = m_RotationForDebris;
        m_SpawnedAnchorImpactImage.transform.Find("HoneyDebris/VelocityBasedDebrisHoney").transform.rotation = m_RotationForDebris;

    }

    //Set destroy timer to the crater object
    private void FadeAndDestroyAnchorImpact()
	{
		//check if one has been spawned yet
		if(m_SpawnedAnchorImpactImage != null)
		{
			//check if it already has a script to destroy itself
			if (m_SpawnedAnchorImpactImage.GetComponent<FadeAndDestroy>() == null)
			{
                m_SpawnedAnchorImpactImage.AddComponent<FadeAndDestroy>();
            }
        }
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
