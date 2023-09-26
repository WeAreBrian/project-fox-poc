using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageVelocityController : MonoBehaviour
{
    [Range(0f, 1f)] public float ExternalInfluencestrength = 0.25f;
    public float EaseInTime = 0.15f;
    public float EaseOutTime = 0.15f;
    public float VelocityThreshold = 5f;

    private int _externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    public void InfluenceGrass(Material mat, float XVelocity)
    {
        mat.SetFloat(_externalInfluence, XVelocity);
    }

    private void Awake()
    {
        // For each child GameObject
        foreach (Transform child in transform)
        {
            // Add BoxCollider2D to the child
            if (child.GetComponent<BoxCollider2D>() == null)
            {
                BoxCollider2D boxCollider = child.gameObject.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
                boxCollider.includeLayers = LayerMask.GetMask("Anchor");
                boxCollider.excludeLayers = ~(LayerMask.GetMask("Anchor"));
            }

            // Add YourCustomScript to the child
            if (child.GetComponent<FoliageVelocityTrigger>() == null)
            {
                child.gameObject.AddComponent<FoliageVelocityTrigger>();
            }
        }
    }
}
