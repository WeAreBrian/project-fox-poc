using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject m_WoodExplodeVFX;

    [SerializeField]
    private GameObject m_WoodVFX;

    [SerializeField]
    private GameObject m_Plank;

    private Collider2D m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
    }

    private Quaternion DetermineImpactDirection(Vector3 rbVelocity)
    {
        // If the object is rotated, compensate for rotation
        float objectRotation = transform.localEulerAngles.z;
        Quaternion invertedRotation = Quaternion.Euler(0f, 0f, -objectRotation);

        // Convert velocity vector to local space
        Vector2 localRbVelocity = invertedRotation * rbVelocity;

        // Compare local positions to determine front or back
        if (localRbVelocity.x > 0)
        {
            //Debug.Log("Hit from the back in local space.");
            return Quaternion.Euler(0, 0, 0);
        }
        else
        {
            //Debug.Log("Hit from the front in local space.");
            return Quaternion.Euler(0, 180, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Anchor"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            m_WoodVFX.transform.localRotation = DetermineImpactDirection(new Vector3(rb.velocity.x, rb.velocity.y, 0));

            m_Collider.enabled = false;
            m_Plank.SetActive(false);
            m_WoodExplodeVFX.SetActive(true);
        }
    }
}
