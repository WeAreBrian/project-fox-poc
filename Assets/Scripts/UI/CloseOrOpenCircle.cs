using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CloseOrOpenCircle : MonoBehaviour
{
    public float m_ShrinkSpeed = 30f; // The speed at which the parent will shrink
    public KeyCode m_TriggerKey = KeyCode.Space; // The key that will trigger the shrink effect

    private Transform m_ChildTransform;
    private Vector3 m_ChildInitialWorldScale;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount == 0)
        {
            Debug.LogWarning("This GameObject has no child. The script won't do anything.");
            return;
        }

        // Assuming the first child is the one to keep the same relative scale
        m_ChildTransform = transform.GetChild(0);
        m_ChildInitialWorldScale = m_ChildTransform.lossyScale * 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Trigger the shrinking effect when the specified key is pressed
        if (Input.GetKeyDown(m_TriggerKey))
        {
            StartCoroutine(ShrinkParentObject());
        }
    }

    public IEnumerator ShrinkParentObject()
    {
        while (transform.localScale.x > 0.01f)
        {
            // Shrink the parent
            transform.localScale -= Vector3.one * Time.deltaTime * m_ShrinkSpeed;

            // Calculate the new local scale for the child to maintain its world scale
            Vector3 newLocalScale = new Vector3(
                m_ChildInitialWorldScale.x / transform.localScale.x,
                m_ChildInitialWorldScale.y / transform.localScale.y,
                m_ChildInitialWorldScale.z / transform.localScale.z
            );

            // Apply the new local scale to the child
            m_ChildTransform.localScale = newLocalScale;

            yield return null;
        }

        // Set the parent scale to (almost) zero at the end
        transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

        // Keep child's world scale intact when parent is at zero
        m_ChildTransform.localScale = m_ChildInitialWorldScale / 0.001f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
