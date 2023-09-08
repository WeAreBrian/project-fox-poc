using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParalax : MonoBehaviour
{

    [SerializeField]
    private float m_relativeMove = .3f;
    private float m_relativeMoveX; 
    private float m_relativeMoveY;

	[SerializeField] bool m_lockY;

	private PositionDelta m_camera;

	// Start is called before the first frame update
	void Start()
    {
		m_camera = Camera.main.GetComponent<PositionDelta>();
		m_relativeMoveX = m_relativeMove;
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
		if (Time.frameCount == 1) return;
		Vector3 move = new Vector3(m_camera.Delta.x * -m_relativeMoveX, m_camera.Delta.y * -m_relativeMoveY, 0);

		transform.position += move;

	}
}
