using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystemDown;
    [SerializeField]
    private ParticleSystem particleSystemRise;

    [Header("Raycast Settings")]
    [SerializeField]
    private Vector3 downRaycastOffset;  // Offset for the down raycast
    [SerializeField]
    private Vector3 upRaycastOffset;    // Offset for the up raycast
    [SerializeField]
    private float raycastDistance;      // How far the ray should check
    [SerializeField]
    private LayerMask waterLayer;       // Assign the layer mask for "Water" in the inspector

    private bool wasDownRaycastHit = false;
    private bool wasUpRaycastHit = false;

    private void Update()
    {
        // Calculate start positions based on the offsets
        Vector3 downRaycastStartPos = transform.position + downRaycastOffset;
        Vector3 upRaycastStartPos = transform.position + upRaycastOffset;

        // Raycasting downwards
        RaycastHit2D downHit = Physics2D.Raycast(downRaycastStartPos, Vector2.down, raycastDistance, waterLayer);
        Debug.DrawRay(downRaycastStartPos, Vector2.down * raycastDistance, Color.blue); // Drawing the ray in Scene view
        if (downHit.collider != null && !wasDownRaycastHit)
        {
            Debug.Log("Down raycast hit successful!");
            particleSystemDown.Play();
            wasDownRaycastHit = true;
        }
        else if (downHit.collider == null)
        {
            wasDownRaycastHit = false;
        }

        // Raycasting upwards
        RaycastHit2D upHit = Physics2D.Raycast(upRaycastStartPos, Vector2.up, raycastDistance, waterLayer);
        Debug.DrawRay(upRaycastStartPos, Vector2.up * raycastDistance, Color.red); // Drawing the ray in Scene view
        if (upHit.collider != null)
        {
            wasUpRaycastHit = true;
        }
        else if (wasUpRaycastHit) // Checks if the raycast was previously hitting but now it's not.
        {
            Debug.Log("Up raycast hit no longer detected!");
            particleSystemRise.Play();
            wasUpRaycastHit = false;
        }
    }
}

