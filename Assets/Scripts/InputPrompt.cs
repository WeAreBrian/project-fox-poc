using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputPrompt : MonoBehaviour
{
	public InteractableObject Interactable;
	[HideInInspector]
	public Vector2 Offset;

	private CanvasGroup m_CanvasGroup;
	[SerializeField]
	private Image m_Button;

	private void Awake()
	{
		m_CanvasGroup = GetComponent<CanvasGroup>();
	}

	private void LateUpdate()
	{
		transform.position = Camera.main.WorldToScreenPoint((Vector2)Interactable.GameObject.transform.position + Offset);
	}

	private void OnEnable()
	{
		LeanTween.cancel(gameObject);
		LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.1f)
			.setFrom(0);

		LeanTween.cancel(m_Button.gameObject);
		LeanTween.move(m_Button.rectTransform, Vector3.zero, 0.2f)
			.setFrom(new Vector3(0, -40, 0))
			.setEaseOutExpo();
	}

	public void Dispose()
	{
		LeanTween.cancel(gameObject);
		LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.1f);

		LeanTween.cancel(m_Button.gameObject);
		LeanTween.move(m_Button.rectTransform, new Vector3(0, -40, 0), 0.2f)
			.setEaseOutExpo();

		Destroy(gameObject, 0.2f);
	}
}
