using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Grounded : MonoBehaviour
{
    public UnityEvent Landed;
    public UnityEvent HitHazard;


    public bool CanJump
    {
        get
        {
            if (OnGround) return true;
            if (OnForcefield && m_AnchorHolder.Surfing) return true;
            return false;
        }
    }
    public bool OnGround { get; private set; }
    public bool OnForcefield { get; private set; }
    private bool m_GroundedLastFrame;

    [SerializeField]
    private LayerMask m_GroundMask;
    [SerializeField]
    private LayerMask m_HazardMask;
    [SerializeField]
    private LayerMask m_ForcefieldMask;
    private const float k_EdgeOffset = 0.4f;
    private const float k_Height = 0.2f;
    private Collider2D m_Collider;
    [SerializeField]
    private AudioClip m_LandSound;
    private float m_ColliderOffset = 0.25f;

    private AnchorHolder m_AnchorHolder;
    [SerializeField]
    private GameObject m_LandingDustPoof;
    [SerializeField]
    private float m_LandingDustPoofPlaybackSpeed = 2f;
    [SerializeField]
    private Vector3 m_LandingDustPoofPosition = new Vector3(0,0,0);
    [SerializeField]
    private Vector3 m_LandingDustPoofScale = new Vector3(1,1,1);
    private AnimationPrefabSpawner m_AnimationPrefabHolder;


    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
        m_AnchorHolder = GetComponent<AnchorHolder>();
        m_AnimationPrefabHolder = GetComponent<AnimationPrefabSpawner>();

    }

    private void FixedUpdate()
    {
        var playerBottom = (Vector2)transform.position - new Vector2(0, m_Collider.bounds.extents.y-m_ColliderOffset);
        
        var playerWidth = m_Collider.bounds.extents.x;
        var boxSize = new Vector2(2 * playerWidth - k_EdgeOffset, k_Height);

        if (!OnGround && Physics2D.OverlapBox(playerBottom, boxSize, 0, m_GroundMask))
        {
            AudioController.PlaySound(m_LandSound, 3, 0.7f, MixerGroup.SFX);
        }
        m_GroundedLastFrame = OnGround;
        OnGround = Physics2D.OverlapBox(playerBottom, boxSize, 0, m_GroundMask);
        var hitHazard = Physics2D.OverlapBox(playerBottom, boxSize, 0, m_HazardMask);
        OnForcefield = Physics2D.OverlapBox(playerBottom, boxSize, 0, m_ForcefieldMask);
        if (!m_GroundedLastFrame)
        {
            if ( OnGround)
            {
                Landed.Invoke();

                //Spawn animation prefab using the script
                m_AnimationPrefabHolder.SpawnAnimationPrefab(m_LandingDustPoof, m_LandingDustPoofPlaybackSpeed, m_LandingDustPoofPosition, m_LandingDustPoofScale);
            }
            else if (hitHazard)
            {
                HitHazard.Invoke();
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        var collider = GetComponent<Collider2D>();
        var playerBottom = (Vector2)transform.position - new Vector2(0, collider.bounds.extents.y-m_ColliderOffset);
        var playerWidth = collider.bounds.extents.x;
        var boxSize = new Vector2(2 * playerWidth - k_EdgeOffset, k_Height);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerBottom, boxSize);
    }
}
