using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LayerParallax : MonoBehaviour
{
    public Transform ReferenceTransform;
    public float SpeedMultiplier;
    private PositionDelta camera;

    private void Awake()
    {

        foreach (LayerParallax p in GetComponentsInChildren<LayerParallax>().Skip(1))
        {
            p.SpeedMultiplier += SpeedMultiplier;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.GetComponent<PositionDelta>();
        transform.localScale = transform.localScale * (1 + SpeedMultiplier);
    }

    // Update is called once per frame
    void Update()
    {
        //skip first frame because it seems to take the position from origin as the cameras delta for the first frame
        if (Time.frameCount == 1) return;

        transform.position += camera.Delta * (SpeedMultiplier);
    }
}
