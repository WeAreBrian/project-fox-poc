using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneLantern : MonoBehaviour
{
    public GameObject lightMask;
    public int size;
    public float timeToFullSize;
    private float growTimer;


    // Update is called once per frame
    void Update()
    {
        if (growTimer > 0 && lightMask.transform.localScale.x < size)
        {
            growTimer -= Time.deltaTime;
            var scale = Mathf.Lerp(0, size, timeToFullSize - growTimer);
            lightMask.transform.localScale = new Vector3(scale, scale, scale);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Anchor"))
        {
            growTimer = timeToFullSize;
        }
    }
}
