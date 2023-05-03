using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    private Rigidbody2D rb;
    private HingeJoint2D joint;
    private AudioSource audioSource;
    private ChainClimber chainMovement;
    public float velocityThreshold;
    public float angularChangeThreshold;
    public int index;

    private float tensionLastFrame;
    public float TensionDifference => joint.reactionForce.magnitude - tensionLastFrame;

    public AudioClip chainClinkSound;
    public AudioClip chainLandSound;
    public AudioClip chainSlideSound;

    [SerializeField]
    private float soundCooldown;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<HingeJoint2D>();
        audioSource = GetComponent<AudioSource>();
        chainMovement = GameObject.Find("PlayerFox").GetComponent<ChainClimber>();
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

        var angle = Vector2.Dot(collision.contacts[0].normal, rb.velocity);

        

        var linearDifference = rb.velocity.magnitude;
        if (linearDifference < velocityThreshold) return;

        if (angle < 12)
        {
            PlaySound(chainSlideSound, 0.15f * (linearDifference / velocityThreshold), 1.1f - Random.Range(0, 0.2f));
        }
        else
        {
            PlaySound(chainLandSound, 0.5f * (linearDifference / velocityThreshold), 1.1f - Random.Range(0, 0.2f));
        }
        soundCooldown = 0.1f;
  
    }
    
    private void ChainInteract()
    {
        float absDifference = Mathf.Abs(TensionDifference);
        
        if (absDifference < angularChangeThreshold) return;

        if (chainMovement.Mounted && chainMovement.LinkIndex < index) return;

        PlaySound(chainClinkSound,(absDifference / angularChangeThreshold) * Random.Range(0.1f, 0.2f), 1.01f - Random.Range(0, 0.02f));
    }

    private void PlaySound(AudioClip clip, float volume, float pitch)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.clip = clip;
        audioSource.Play();
    }

}
