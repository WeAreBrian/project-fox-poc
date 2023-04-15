using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Chain : MonoBehaviour
{
	public event Action LinksCreated;
	public Vector2 LinkSize = new Vector2(0.3f, 0.9f);
	public float LinkMass = 0.1f;
	public Sprite LinkSprite;
	public Transform From;
	public Transform To;
	public float MaxLength = 20;
	[Tooltip("The priority of this chain when mounting. Higher priority chains will be mounted first.")]
	public int MountPriority;


	public LayerMask ignoreMask;
	private List<Vector2> pathTo = new List<Vector2>();

	/// <summary>
	/// The distance between two HingeJoint2D anchors on the same link.
	/// </summary>
	public float LinkAnchorDistance => LinkSize.y - LinkSize.x;
	public float LinkAnchorOffset => LinkAnchorDistance / 2;
	public Rigidbody2D[] Links => m_Links;
	public float Length => m_Links.Length * LinkAnchorDistance;
	private Rigidbody2D[] m_Links;

	private void Start()
	{
		CreateChain(From.position, To.position);
	}
	private void FixedUpdate()
	{
		pathTo = RayToNextPendulumPoint(From.transform.position, new List<Vector2>() { From.transform.position });
	}

	private void CreateChain(Vector2 fromPoint, Vector2 toPoint)
	{
		var direction = (toPoint - fromPoint).normalized;
		var rotation = Quaternion.FromToRotation(Vector2.up, direction);
		var distance = Mathf.Min(Vector2.Distance(fromPoint, toPoint), MaxLength);
		var links = Mathf.CeilToInt(distance / LinkAnchorDistance);

		m_Links = new Rigidbody2D[links];

		for (var i = 0; i < m_Links.Length; i++)
		{
			var link = CreateLink();

			link.name = $"Link{i}";
			link.transform.parent = transform;
			link.transform.rotation = rotation;
			link.transform.position = fromPoint + direction * i * LinkAnchorDistance;

			m_Links[i] = link;
		}

		for (var i = 1; i < m_Links.Length; i++)
		{
			ConnectLink(m_Links[i], m_Links[i - 1]);
		}

		LinksCreated?.Invoke();
	}

	public void UpdateChainLinksMass(float mass)
	{
		foreach (Rigidbody2D link in Links)
		{
			link.mass = mass;
		}
	}

	public Vector2 Tug(GameObject targetObject)
	{
		var distance = Vector2.Distance(To.position, From.position);
		var index = Mathf.Clamp(Mathf.RoundToInt(distance / LinkAnchorDistance-2), 0, Links.Length-1);
		var link = Links[index];
		link.position = To.position;

		//This is mainly for visuals, still needs a lot of testing...
		for (int i = 0; i < Links.Length; i++)
		{
			Links[i].velocity = Vector2.zero;
		}
		if (targetObject.CompareTag("Player"))
		{
			return pathTo.Last();
		}
		else
		{
			if (pathTo.Count < 2)
			{
				return To.position;
			}
			return pathTo[1];
		}
	}

	private Rigidbody2D CreateLink()
	{
		var link = new GameObject();
		link.layer = LayerMask.NameToLayer("Chain");

		var spriteRenderer = link.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = LinkSprite;

		var rigidBody = link.AddComponent<Rigidbody2D>();
		rigidBody.mass = LinkMass;

		var collider = link.AddComponent<CapsuleCollider2D>();
		collider.size = LinkSize;

		return rigidBody;
	}

	private void ConnectLink(Rigidbody2D link, Rigidbody2D previousLink)
	{
		var hingeJoint = link.AddComponent<HingeJoint2D>();
		hingeJoint.connectedBody = previousLink;
		hingeJoint.autoConfigureConnectedAnchor = false;
		hingeJoint.connectedAnchor = new Vector2(0, LinkAnchorOffset);
		hingeJoint.anchor = new Vector2(0, -LinkAnchorOffset);
	}
	private void OnDrawGizmos()
	{
		for (int i = 0; i <pathTo.Count - 1; i++)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(pathTo[i],pathTo[i + 1]);
		}
		Gizmos.DrawLine(pathTo.Last(), To.position);

		foreach (Rigidbody2D link in Links)
		{
			if (link.bodyType == RigidbodyType2D.Static)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(link.position, 0.2f);
			}
		}
	}

	private List<Vector2> RayToNextPendulumPoint(Vector2 startingPoint, List<Vector2> path)
	{
		var dir = (To.position - (Vector3)startingPoint).normalized;
		startingPoint = (Vector3)startingPoint + (dir * 0.01f);
		RaycastHit2D hit = Physics2D.Raycast(startingPoint, dir, 50, ~(ignoreMask));
		if (hit.collider != null)
		{
			Vector2 closestCorner;
			if (!hit.collider.CompareTag("Player"))
			{

				closestCorner = GetCorner(hit, path);
				Debug.Log("going Deeper");
				path.Add(closestCorner);
				RayToNextPendulumPoint(closestCorner, path);
			}
		}
		return path;
	}

	private Vector2 GetCorner(RaycastHit2D hit, List<Vector2> path)
	{
		List<Vector2> corners = new List<Vector2>()
		{
			new Vector2(hit.collider.bounds.min.x, hit.collider.bounds.max.y),
			new Vector2(hit.collider.bounds.max.x, hit.collider.bounds.max.y),
			new Vector2(hit.collider.bounds.min.x, hit.collider.bounds.min.y),
			new Vector2(hit.collider.bounds.max.x, hit.collider.bounds.min.y)
		};

		return corners.Except(path).OrderBy(x => Vector2.Distance(hit.point, x)).First();
	}

	public void Stiffen()
	{
		Release();
		for (int i = 1; i < pathTo.Count; i++)
		{
			var link = Links.OrderBy(x => Vector2.Distance(pathTo[i], x.position)).First();
			link.bodyType = RigidbodyType2D.Static;
		}
	}

	public void Release()
	{
		foreach (Rigidbody2D link in Links)
		{
			link.bodyType = RigidbodyType2D.Dynamic;
		}
	}
}
