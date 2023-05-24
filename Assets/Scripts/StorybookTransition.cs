using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorybookTransition : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;

	private void Start()
	{
		//var orthographicProjection = m_Camera.projectionMatrix;
		//var perspectiveProjection = Matrix4x4.Perspective(m_Camera.fieldOfView, m_Camera.aspect, m_Camera.nearClipPlane, m_Camera.farClipPlane);

		//LeanTween.value(gameObject, t => m_Camera.projectionMatrix = Lerp(orthographicProjection, perspectiveProjection, t), 0, 1, 2).setLoopPingPong();
	}

	private static Matrix4x4 Lerp(Matrix4x4 a, Matrix4x4 b, float t)
	{
		var result = new Matrix4x4();

		for (var i = 0; i < 16; i++)
		{
			result[i] = Mathf.Lerp(a[i], b[i], t);
		}

		return result;
	}
}
