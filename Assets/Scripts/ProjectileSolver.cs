using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;

public static class ProjectileSolver
{
	/// <summary>
	/// Find the angle required to hit the target.
	/// </summary>
	/// <param name="position">The starting position of the projectile.</param>
	/// <param name="target">The target position of the projectile.</param>
	/// <param name="speed">The magnitude of the initial velocity.</param>
	/// <param name="angle">The angle of the initial velocity.</param>
	public static bool CalculateAngle(Vector2 position, Vector2 target, float speed, out float angle)
	{
		var displacement = target - position;
		var horizontalDistance = displacement.x;
		var verticalDistance = displacement.y;
		var horizontalSpeed = speed;
		var gravity = Physics2D.gravity.y;

		// Calculate the initial vertical speed needed to hit the target
		var a = -0.5f * gravity;
		var b = verticalDistance;
		var c = -horizontalDistance;
		var discriminant = b * b - 4f * a * c;

		if (discriminant < 0)
		{
			angle = 0;
			return false;
		}

		var root = Mathf.Sqrt(discriminant);
		var timeToTarget = Mathf.Max((-b + root) / (2 * a), (-b - root) / (2 * a));
		var verticalSpeed = -gravity * timeToTarget;

		// Calculate the firing angle
		angle = Mathf.Atan2(verticalSpeed, horizontalSpeed) * Mathf.Rad2Deg;

		return true;
	}
}
