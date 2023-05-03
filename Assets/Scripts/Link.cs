using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    private Rigidbody2D rb;
    private HingeJoint2D joint;
    private AudioSource audioSource;
    public float velocityThreshold;
    public float angularChangeThreshold;

    private float tensionLastFrame;
    public float TensionDifference => joint.reactionForce.magnitude - tensionLastFrame;

    public AudioClip chainClinkSound;
    public AudioClip chainLandSound;

    [SerializeField]
    private float soundCooldown;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<HingeJoint2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        soundCooldown -= Time.deltaTime;
        if (joint != null)
        {
            ChainInteract();
            tensionLastFrame = joint.reactionForce.magnitude;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (soundCooldown > 0) return;

        Rigidbody2D other = collision.gameObject.GetComponent<Rigidbody2D>();

        if (other == null) return;
        
        var linearDifference = (rb.velocity - other.velocity).magnitude;
        if (linearDifference < velocityThreshold) return;

        PlaySound(chainLandSound, 0.5f * (linearDifference/velocityThreshold), 1.1f - Random.Range(0, 0.2f));
        soundCooldown = 0.1f;
  
    }
    
    private void ChainInteract()
    {
        float absDifference = Mathf.Abs(TensionDifference);
        
        Debug.Log(absDifference);
        if (absDifference < angularChangeThreshold) return;

        PlaySound(chainClinkSound, 0.2f * (absDifference / angularChangeThreshold), 1.1f - Random.Range(0, 0.2f));
    }

    private void PlaySound(AudioClip clip, float volume, float pitch)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.clip = clip;
        audioSource.Play();
    }

}
