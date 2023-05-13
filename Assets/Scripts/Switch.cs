using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject m_ConnectedObject;
    [SerializeField]
    private Sprite m_UntoggledSprite;
    [SerializeField]
    private Sprite m_ToggledSprite;
	[SerializeField]
	private InputActionReference m_Input;
	private SpriteRenderer m_Renderer;
    private bool m_Togglable;

    public InputActionReference Input => m_Input;

	private void Start()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Renderer.sprite = m_UntoggledSprite;
        m_Togglable = true;
        m_ConnectedObject.GetComponent<IToggle>().DisableSelfToggle();
    }

    private void Toggle()
    {
        if (!m_Togglable) return;

        var toggle = m_ConnectedObject.GetComponent<IToggle>();
        toggle.Toggle();

        m_Togglable = false;
        m_Renderer.sprite = m_ToggledSprite;

        StartCoroutine(ResetState(toggle.GetResetTime()));
    }

    public IEnumerator ResetState(float delay)
    {
        yield return new WaitForSeconds(delay);

        m_Togglable = true;
        m_Renderer.sprite = m_UntoggledSprite;
    }

	public void Interact()
	{
        Toggle();
	}
}
