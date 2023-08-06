using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Recall : MonoBehaviour
{
	private RecallSlingshot m_RecallSlingshot;

	public delegate void Trigger();
	public static event Trigger activate;

	private Anchor m_Anchor;
	private AnchorHolder m_Holder;


    [SerializeField]
	private AudioClip m_RecallSound;

	private void Awake()
	{
		m_RecallSlingshot = GetComponent<RecallSlingshot>();

		m_Anchor = FindObjectOfType<Anchor>();
		m_Holder = GetComponent<AnchorHolder>();

    }



    private void OnRecall()
	{
		//Need to activate the slingshot BEFORE the anchor moves!
        m_RecallSlingshot?.TrySlingshot();


		AudioController.PlaySound(m_RecallSound, 1, 1, MixerGroup.SFX);
		m_Anchor.transform.position = transform.position;
		m_Holder.ForcePickup();
	}
}
