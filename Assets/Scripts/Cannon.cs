using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, IInsertable, IMovable
{
	[Tooltip("Align the cannon to hit a target position. Can be left blank.")]
    public Transform Target;
	public float LaunchSpeed = 20;
	public float Fuse = 1;

	public bool HasProjectile => m_Projectile != null;
	public Vector2 LaunchVelocity => m_Barrel.transform.up * LaunchSpeed;

	[SerializeField]
	private GameObject m_Barrel;
	private Rigidbody2D m_Projectile;

	public void Fire()
	{
		if (m_Projectile == null)
		{
			return;
		}

		m_Projectile.isKinematic = false;
		m_Projectile.velocity = LaunchVelocity;
		m_Projectile = null;

		LeanTween.cancel(m_Barrel);
		LeanTween.scale(m_Barrel, Vector3.one, 0.2f)
			.setFrom(Vector3.one * 1.1f)
			.setEaseOutBack();
	}

	public void Load(Rigidbody2D projectile)
	{
		if (m_Projectile != null)
		{
			return;
		}

		m_Projectile = projectile;
		m_Projectile.isKinematic = true;
		m_Projectile.angularVelocity = 0;
		m_Projectile.velocity = Vector2.zero;
		m_Projectile.MovePosition(m_Barrel.transform.position);

		LeanTween.cancel(m_Barrel);
		LeanTween.scale(m_Barrel, Vector3.one, 0.2f)
			.setFrom(Vector3.one * 1.1f)
			.setEaseOutBack();

		LeanTween.cancel(gameObject);
		LeanTween.delayedCall(gameObject, Fuse, Fire);
	}

	private void Update()
	{
		if (Target != null)
		{
			AimAtTarget();
		}
	}

	private void AimAtTarget()
	{
		if (ProjectileSolver.CalculateAngle(m_Barrel.transform.position, Target.position, LaunchSpeed, out var launchAngle))
		{
			m_Barrel.transform.localEulerAngles = new Vector3(0, 0, -launchAngle);
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Anchor"))
		{
			Load(collider.GetComponent<Rigidbody2D>());
		}
	}

	private void OnDrawGizmos()
	{
		if (Target != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(m_Barrel.transform.position, Target.position);
		}
	}

	public void Insert(GameObject go)
	{
		var rigidBody = go.GetComponent<Rigidbody2D>();

		if (rigidBody != null)
		{
			Load(rigidBody);
		}
	}
}
