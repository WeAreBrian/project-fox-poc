using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpeedLines : MonoBehaviour
{

    [SerializeField]
    private float minSpeedThreshold = 18.0f; // The speed below which the image will be fully transparent
    [SerializeField]
    private float maxSpeedThreshold = 50.0f; // The speed above which the image will be fully opaque
    [SerializeField]
    private float maxOpacity = 0.02f;
    [SerializeField]
    private Image uiImage;

    [SerializeField]
    private Sprite[] sprites; // Array of sprites to cycle through
    [SerializeField]
    private float spriteChangeInterval = 0.1f; // Time interval between sprite changes

    [SerializeField]
    private float speedLinelerpSpeed = 0.1f;    //Lerp speed for the opacity changes

    private GameObject playerFox;
    private Rigidbody2D rb2d;
    private int currentSpriteIndex = 0;
    private float spriteChangeTimer = 0.0f;
    private float newAlpha = 0.0f;


    //image is hidden by default so its not in the way in the scene view
    private void Awake()
    {
        uiImage.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerFox = GameObject.FindGameObjectWithTag("Player");
        rb2d = playerFox.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Get the magnitude of the velocity
        float speed = rb2d.velocity.magnitude;

        // Calculate the new alpha value based on the speed
        float targetAlpha = 0.0f;
        if (speed >= maxSpeedThreshold)
        {
            targetAlpha = maxOpacity;
        }
        else if (speed > minSpeedThreshold)
        {
            targetAlpha = maxOpacity * (speed - minSpeedThreshold) / (maxSpeedThreshold - minSpeedThreshold);
        }
        

        // Smoothly approach the new alpha value using Lerp
        newAlpha = Mathf.Lerp(newAlpha, targetAlpha, speedLinelerpSpeed);

        // Set the new alpha value
        Color newColor = uiImage.color;
        newColor.a = newAlpha;
        uiImage.color = newColor;

        // Cycle sprite image
        if (sprites.Length > 0)
        {
            spriteChangeTimer += Time.fixedDeltaTime;
            if (spriteChangeTimer >= spriteChangeInterval)
            {
                currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length;
                uiImage.sprite = sprites[currentSpriteIndex];
                spriteChangeTimer = 0.0f;
            }
        }


        ////Trails
        //if (speed > minSpeedThreshold)
        //{
        //    //SpawnSpeedTrail();
        //    float targedSpeedLineLife = trailingSpeedLineSizeMultiplier * (speed - minSpeedThreshold) / (maxSpeedThreshold - minSpeedThreshold);
        //    float newSpeedLineLife = Mathf.Lerp(newAlpha, targedSpeedLineLife, trailingSpeedLinelerpSpeed);
        //    if (speedLineTrailIsCurrentlyAChild == false)
        //    {
        //        lastSpeedLineTrailSpawned = Instantiate(speedLineTrail, playerFox.transform.position, Quaternion.identity);
        //        lastSpeedLineTrailSpawned.transform.SetParent(playerFox.transform, true);

        //        speedLineTrailIsCurrentlyAChild = true;
        //    }
        //    lastSpeedLineTrailSpawned.GetComponent<TrailRenderer>().time = newSpeedLineLife;
        //}
        //else if (speed <= minSpeedThreshold)
        //{
        //    //RemoveSpeedTrail();
        //    lastSpeedLineTrailSpawned.GetComponent<TrailRenderer>().time = 0f;

        //}

        //Debug.Log(lastSpeedLineTrailSpawned.GetComponent<TrailRenderer>().time);
    }

    //private void SpawnSpeedTrail()
    //{
    //    if (speedLineTrailIsCurrentlyAChild == false)
    //    {
    //        lastSpeedLineTrailSpawned = Instantiate(speedLineTrail, playerFox.transform.position, Quaternion.identity);
    //        lastSpeedLineTrailSpawned.transform.SetParent(playerFox.transform, true);

    //        speedLineTrailIsCurrentlyAChild = true;
    //    }
        
    //}

    //private void RemoveSpeedTrail()
    //{
    //    if (speedLineTrailIsCurrentlyAChild == true)
    //    {
    //        // Unparent this GameObject
    //        lastSpeedLineTrailSpawned.transform.parent = null;

    //        //Destroy(lastSpeedLineTrailSpawned, lastSpeedLineTrailSpawned.GetComponent<TrailRenderer>().time);
    //        speedLineTrailIsCurrentlyAChild = false; ;
    //    }
    //}
}
