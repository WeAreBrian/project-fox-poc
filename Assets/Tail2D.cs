using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail2D : MonoBehaviour
{
    [Header("Tail")]
    [SerializeField] Transform tailAnchor = null;
    [SerializeField] Rigidbody2D tailRigidbody = null;

    private Rigidbody2D controllerRigidbody;
    private bool isFlipped;
    readonly Quaternion flippedRotation = new Quaternion(0, 0, 1, 0);

    private void Awake()
    {
        controllerRigidbody = GetComponent<Rigidbody2D>();

    }

    private void UpdateTailPose()
    {
        // Calculate the extrapolated target position of the tail anchor.
        Vector2 targetPosition = tailAnchor.position;
        targetPosition += controllerRigidbody.velocity * Time.fixedDeltaTime;

        tailRigidbody.MovePosition(targetPosition);

        //Debug.Log("Target Pos:" + targetPosition);
        //Debug.Log("Current Pos:" + tailRigidbody.position);
        //Debug.Log("Current Pos T" + tailRigidbody.transform.position);

        //if (isFlipped)
        //    tailRigidbody.SetRotation(tailAnchor.rotation * flippedRotation);
        //else
        //    tailRigidbody.SetRotation(tailAnchor.rotation);
    }

    private void FixedUpdate()
    {
        UpdateTailPose();
    }
}









//{
//	private GameObject[] m_Children;

//	public GameObject Hook;
//	public GameObject TailBone;

//    //If something goes wrong in the future, animator should be the fox sprite with object with the animator in the scene. Target joint should be the last joint in the tail.
//    [SerializeField] private Animator m_FoxAnimator;
//	[SerializeField] private TargetJoint2D m_TargetJoint = null;
	
//	private void Awake()
//	{
//		// Initialize the array with the children
//		m_Children = new GameObject[transform.childCount];
//		for (int m_Index = 0; m_Index < transform.childCount; m_Index++)
//		{
//			m_Children[m_Index] = transform.GetChild(m_Index).gameObject;
//		}

//		//UnparentAllChildren();
//	}

//    private void Update()
//    {
//        float distance = Vector3.Distance(Hook.transform.position, TailBone.transform.position);

//        // Log the distance to the debug console
//        Debug.Log("Distance between objects: " + distance);
//        //TailBone.GetComponent<Rigidbody2D>().position = Hook.transform.position;
//        //TailBone.GetComponent<Rigidbody2D>().MovePosition(Hook.transform.position);
//        //TailBone.transform.position = Hook.transform.position;


//    }


//    private void FixedUpdate()
//	{
        
//        //Make the targetjoints target be behind the player facing direction
//        m_TargetJoint.target = m_FoxAnimator.transform.position + new Vector3(m_FoxAnimator.GetFloat("Facing") * -1000, m_FoxAnimator.transform.position.y);

//        //camera is the hook
//        //mainplayer is the tailbone

//        //TailBone.transform.position = Hook.transform.position;
//        //TailBone.GetComponent<Rigidbody2D>().MovePosition(Hook.transform.position);
//        //Get the hook velocity after `AddForce`

//        //Vector3 hookVelocity;
//        //hookVelocity = Hook.GetComponent<Rigidbody2D>().velocity * Time.fixedDeltaTime;

//        //I know the velocity so I know where the hook will end up once the physics engine sets all the transforms

//        //Move fox forward, then back (or set that into a variable and use that)
//        //Hook.transform.position += (Vector3)Hook.GetComponent<Rigidbody2D>().velocity * Time.fixedDeltaTime;

//        //TailBone.GetComponent<Rigidbody2D>().position = (Hook.transform.TransformPoint(Hook.transform.position));
//        //TailBone.transform.position = Hook.transform.position;
//        //TailBone.GetComponent<Rigidbody2D>().position = (new Vector3(0,0,0));
//        //TailBone.GetComponent<Rigidbody2D>().MovePosition(Hook.transform.position);


//        //Hook.transform.position -= (Vector3)Hook.GetComponent<Rigidbody2D>().velocity * Time.fixedDeltaTime;

//        //TailBone.GetComponent<Rigidbdy2D>().MoveRotation(mainPlayer.transform.rotation * rot);

//        //TailBone.transform.position -= hookVelocity;
//    }

//    private void UnparentAllChildren()
//	{
//		// Loop through all children and unparent them
//		foreach (GameObject m_Child in m_Children)
//		{
//			m_Child.transform.SetParent(null);
//		}
//	}
//}
