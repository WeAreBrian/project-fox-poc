using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

// this is disabled for now but not deleted just in case we need it again

public class ChainTugger : MonoBehaviour
{
	public float Cooldown = 1.5f;
	public float PlayerJumpOffset = 0.25f;
	public float PlayerTugSpeed = 10;
	public float AnchorDislodgeTravelTime = 0.85f;
	public float AnchorFreeTravelTime = 0.85f;

	private IdealChain m_Chain;
	private Timer m_Cooldown;
	private Anchor m_Anchor;
	private Grounded m_Grounded;
	private AnchorHolder m_Holder;
	[SerializeField]
	private GameObject floatingText;

	[SerializeField]
	private AudioClip m_PlayerTugSound;
	[SerializeField]
	private AudioClip m_FullTugSound;

	private void Awake()
	{
		m_Chain = FindObjectOfType<IdealChain>();
		m_Anchor = FindObjectOfType<Anchor>();

		m_Cooldown = new Timer(Cooldown);

		m_Grounded = GetComponent<Grounded>();
		m_Holder = GetComponent<AnchorHolder>();
	}

	private void OnTug()
	{
		if (!m_Cooldown.Paused)
		{
			FloatingText f = Instantiate(floatingText).GetComponentInChildren<FloatingText>();
			f.Set("Tug On Cooldown", transform.position + Vector3.up, Color.green);
			return;
		}


		transform.position += new Vector3(0, PlayerJumpOffset);
		var tugvector = m_Chain.PlayerToPendulum * PlayerTugSpeed;
		Debug.Log(tugvector);
		m_Chain.Player.velocity += tugvector;
		AudioController.PlaySound(m_PlayerTugSound, 0.4f, 1, MixerGroup.SFX);

		m_Cooldown.Start();
	}

	private void Update()
	{
		m_Cooldown.Tick();
	}
}
