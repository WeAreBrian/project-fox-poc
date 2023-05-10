using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorStall : MonoBehaviour
{
	[SerializeField]
	private float m_StallTime = 1f;
	[SerializeField]
	private bool m_RevertVelocity = true;   //Set this in inspector to false if you want it to fall after ending stall.
	[SerializeField]
	private float m_Cooldown;
	
	private Rigidbody2D m_AnchorRigidbody;
	private Anchor m_AnchorScript;
	private AnchorHolder m_AnchorHolder;
	private Vector2 m_Velocity;
	private float m_AngularVelocity;
	private bool isStalled;
	private float m_CooldownTimer;
	[SerializeField]
	private GameObject floatingText;
	[SerializeField]
	private InputAction m_AnchorInteract;

	private void Awake()
	{
		//Sets stuff from anchor
		m_AnchorHolder = GetComponent<AnchorHolder>();
		GameObject m_AnchorObject = GameObject.Find("Anchor");
		m_AnchorScript = m_AnchorObject.GetComponent<Anchor>();
		m_AnchorRigidbody = m_AnchorScript.GetComponent<Rigidbody2D>();
		m_AnchorInteract = GetComponent<PlayerInput>().actions["AnchorInteract"];
	}


	private void Update()
	{
		m_CooldownTimer -= Time.deltaTime;
	}

	private void OnAnchorInteract()
	{
		//if fox is not holding the anchor and its not already being stalled.
		if (!m_AnchorHolder.HoldingAnchor && !isStalled && m_AnchorInteract.IsPressed())
		{
			if (m_CooldownTimer < 0)
			{

				//Save values
				isStalled = true;
				m_CooldownTimer = m_Cooldown;
				if (m_RevertVelocity)
				{
					m_Velocity = m_AnchorRigidbody.velocity;
					m_AngularVelocity = m_AnchorRigidbody.angularVelocity;
				}

				//Stall
				m_AnchorRigidbody.bodyType = RigidbodyType2D.Static;
				StartCoroutine(WaitCoroutine(m_StallTime));
			}
			else
			{
				FloatingText f = Instantiate(floatingText).GetComponentInChildren<FloatingText>();
				f.Set("Stall On Cooldown", transform.position + Vector3.up, Color.blue);
				return;
			}
			
			//Stall
			m_AnchorRigidbody.bodyType = RigidbodyType2D.Static;
			m_AnchorScript.ActivateShake(m_StallTime);
			StartCoroutine(WaitCoroutine(m_StallTime));
		}

		//Do this after stall timer
		IEnumerator WaitCoroutine(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			//un-stall after waiting
			m_AnchorRigidbody.bodyType = RigidbodyType2D.Dynamic;
			if (m_RevertVelocity)
			{
				m_AnchorRigidbody.velocity = m_Velocity;
				m_AnchorRigidbody.angularVelocity = m_AngularVelocity;
			}
			isStalled = false;
		}
	}
}
