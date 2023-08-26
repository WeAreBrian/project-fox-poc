using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CloseOrOpenCircle : MonoBehaviour
{
    public float m_Speed = 70f; // The speed at which the parent will shrink or grow
    public KeyCode m_ShrinkKey = KeyCode.Space; // The key that will trigger the shrink effect
    public KeyCode m_GrowKey = KeyCode.G; // The key that will trigger the grow effect

    private Transform m_ChildTransform;
    private Vector3 m_ChildInitialWorldScale;
    private GameObject m_PlayerFox;
    private RectTransform m_RectTransform;
    private Image m_Image;

    public Action OnShrinkComplete;

    private void Awake()
    {
        m_PlayerFox = GameObject.Find("PlayerFox");
        m_RectTransform = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();

        m_Image.enabled = true; //this so it can be disabled in the editor but turn on in play mode
    }

    void Start()
    {

        if (transform.childCount == 0)
        {
            Debug.LogWarning("This GameObject has no child. The script won't do anything. Contact Sach");
            return;
        }

        // Assuming the first child is the one to keep the same relative scale
        m_ChildTransform = transform.GetChild(0);
        m_ChildInitialWorldScale = m_ChildTransform.lossyScale * 2;

        StartCoroutine(GrowParentObject());



    }

    // Update is called once per frame
    void Update()
    {
        // Trigger the shrinking effect when the specified key is pressed
        if (Input.GetKeyDown(m_ShrinkKey))
        {
            StartCoroutine(ShrinkParentObject());
        }

        // Trigger the growing effect when the specified key is pressed
        if (Input.GetKeyDown(m_GrowKey))
        {
            StartCoroutine(GrowParentObject());
        }

        
        
    }

    public IEnumerator ShrinkParentObject(String m_SceneToOpen = null)
    {
        

        while (transform.localScale.x > 0.001f)
        {
            // Shrink the parent
            transform.localScale -= Vector3.one * Time.deltaTime * m_Speed;

            // Calculate the new local scale for the child to maintain its world scale
            Vector3 newLocalScale = new Vector3(
                m_ChildInitialWorldScale.x / transform.localScale.x,
                m_ChildInitialWorldScale.y / transform.localScale.y,
                m_ChildInitialWorldScale.z / transform.localScale.z
            );

            // Apply the new local scale to the child
            m_ChildTransform.localScale = newLocalScale;

            KeepCentredOnFox();

            yield return null;
        }

        // Set the parent scale to (almost) zero at the end
        transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

        // Keep child's world scale intact when parent is at zero
        m_ChildTransform.localScale = m_ChildInitialWorldScale / 0.001f;

        try
        {
            if(m_SceneToOpen == null)
            {
                m_SceneToOpen = SceneManager.GetActiveScene().name;
            }
            SceneManager.LoadScene(m_SceneToOpen);
        }
        catch
        {
            Debug.Log("AY THAT'S NOT A LEGIT SCENENAME BRO");
        }

        OnShrinkComplete?.Invoke(); // Call the callback
        Debug.Log("Fin");
    }

    public IEnumerator GrowParentObject()
    {
        

        // Start from a very small scale
        transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

        // Initial scale for the child to maintain its world scale
        m_ChildTransform.localScale = m_ChildInitialWorldScale / 0.001f;

        while (transform.localScale.x < 25.0f)
        {
            // Grow the parent
            transform.localScale += Vector3.one * Time.deltaTime * m_Speed;

            // Calculate the new local scale for the child to maintain its world scale
            Vector3 newLocalScale = new Vector3(
                m_ChildInitialWorldScale.x / transform.localScale.x,
                m_ChildInitialWorldScale.y / transform.localScale.y,
                m_ChildInitialWorldScale.z / transform.localScale.z
            );

            // Apply the new local scale to the child
            m_ChildTransform.localScale = newLocalScale;

            KeepCentredOnFox();

            yield return null;
        }

        // Set the parent scale to one at the end
        transform.localScale = new Vector3(25,25,1);

        // Reset child's local scale to its initial local scale
        m_ChildTransform.localScale = m_ChildInitialWorldScale / 1.0f;
    }

    private void KeepCentredOnFox()
    {
        // Calculate the position of the UI element based on the object's position
        Vector3 worldPosition = m_PlayerFox.transform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Set the position of the UI element's RectTransform
        m_RectTransform.position = screenPosition;
    }
}
