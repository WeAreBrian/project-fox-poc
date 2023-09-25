using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParalax : MonoBehaviour
{

    [SerializeField]
    [Tooltip("A value between 1 and -1.\nPut negative value for foreground.\nThings further away should have a value closer to 0.")]
    private float m_relativeMove = .3f;
    private float m_relativeMoveX; 
    private float m_relativeMoveY;
    [SerializeField]
    private bool m_distanceCheck;
    private GameObject m_player;
    [SerializeField]
    private float m_distance;

	[SerializeField] bool m_lockY;

	private PositionDelta m_camera;

    private bool m_ScriptLoaded = true;

    // Start is called before the first frame update
    void Awake()
    {
        // gets cam velocity
		m_camera = Camera.main.GetComponent<PositionDelta>();

		m_relativeMoveX = m_relativeMove;

        m_player = GameObject.FindGameObjectWithTag("Player");

        // make y movement 0 if locked
        if (m_lockY)
        {
            m_relativeMoveY = 0;
        }
        else
        {
            m_relativeMoveY = m_relativeMove; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        //The parallaxs ends up offset of the first frame isnt skipped. This if statement makes it not execute on first frame only.
        if (m_ScriptLoaded)
        {
            m_ScriptLoaded = false;
            return;
        }

        // calculate movement

        if (m_distanceCheck)
        {
            float dist = Vector2.Distance(gameObject.transform.position, m_player.transform.position); 

            if(dist < m_distance) { }

        }
		Vector3 move = new Vector3(m_camera.Delta.x * -m_relativeMoveX, m_camera.Delta.y * -m_relativeMoveY, 0);

		transform.position += move;
	}
}
