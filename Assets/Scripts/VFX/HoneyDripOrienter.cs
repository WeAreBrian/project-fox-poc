using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneyDripOrienter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // This assumes the object's local Y-axis is its "up" direction.
        Vector3 currentUp = transform.up;

        // Calculate the rotation required to align the object's up with the world's up
        Quaternion targetRotation = Quaternion.FromToRotation(currentUp, Vector3.up);

        // Apply this rotation to the object
        transform.rotation = targetRotation * transform.rotation;
    }
}
