using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AnchorTrajectory : MonoBehaviour
{
    public int Points = 32;
    public float TimeStep = 0.2f;
    public Vector2 Velocity;

    [SerializeField]
    private LayerMask m_GroundMask;
    private LineRenderer m_LineRenderer;
    private Vector2[] m_Path;

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_Path = new Vector2[Points];
    }

    private void FixedUpdate()
    {
        SimulateTrajectory(Velocity, TimeStep, m_Path);
        var pathOffset = TrimTrajectory(m_Path);
        ShowTrajectory(m_Path, pathOffset);
    }

    /// <summary>
    /// Simulates the path of motion for the anchor using simple projectile physics
    /// </summary>
    /// <param name="velocity">Initial velocity of anchor</param>
    /// <param name="timeStep">Time step interval</param>
    /// <param name="path">Output of points along trajectory</param>
    private void SimulateTrajectory(Vector2 velocity, float timeStep, Vector2[] path)
    {
        path[0] = Vector2.zero;

        for (var i = 1; i < path.Length; i++)
        {
            path[i] = path[i - 1] + velocity * timeStep;
            velocity += Physics2D.gravity * timeStep;
        }
    }

    /// <summary>
    /// Trims the trajectory to the first point that collides with the ground
    /// </summary>
    /// <param name="path">Points along trajectory</param>
    /// <returns>The index and distance offset of the point that first touches the ground</returns>
    private float TrimTrajectory(Vector2[] path)
    {
        for (var pathIndex = 0; pathIndex < path.Length - 1; pathIndex++)
        {
            var point = path[pathIndex];
            var nextPoint = path[pathIndex + 1];

            var direction = (nextPoint - point).normalized;
            var distance = Vector2.Distance(point, nextPoint);

            var hit = Physics2D.Raycast(point, direction, distance, m_GroundMask);

            if (hit)
            {
                var collisionOffset = hit.distance / distance;
                return pathIndex + collisionOffset;
            }
        }

        return path.Length - 1;
    }

    /// <summary>
    /// Updates the line renderer to display the anchor's truncated trajectory
    /// </summary>
    /// <param name="path">Points along trajectory</param>
    /// <param name="pathOffset">The index and distance offset of the last point</param>
    private void ShowTrajectory(Vector2[] path, float pathOffset)
    {
        var trimmedPathLength = Mathf.CeilToInt(pathOffset);
        var lineLength = trimmedPathLength + 1;

        m_LineRenderer.positionCount = lineLength;
        for (var i = 0; i < trimmedPathLength; i++)
        {
            m_LineRenderer.SetPosition(i, path[i]);
        }

        var lastPoint = path[trimmedPathLength - 1];
        var lastPointOffset = pathOffset - trimmedPathLength + 1;
        var lastPointPosition = lastPoint + (path[trimmedPathLength] - lastPoint) * lastPointOffset;

        m_LineRenderer.SetPosition(lineLength - 1, lastPointPosition);
    }
}
