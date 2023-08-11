using UnityEngine;

/// <summary>
/// Dynamically update the scaling of particle system's shape according to its container's scaling (Only runs in edit mode).
/// </summary>
[ExecuteInEditMode]
public class DynamicVFXShape : MonoBehaviour
{
    [SerializeField]
    private Transform m_ContainerTransform;

    [SerializeField]
    private ParticleSystem m_ParticleSystem;

    private void Start()
    {
        if (m_ParticleSystem == null)
            Debug.LogError($"Object {gameObject.name} is missing a particle system component. Please assign a particle system component.");

        if (m_ContainerTransform == null)
            Debug.LogWarning($"Object {gameObject.name} doesn't have a parent object to adjust its shape to.");
    }

    private void Update()
    {
        if (m_ContainerTransform == null || m_ParticleSystem == null)
            return;

        UpdateBoxLength();
    }

    private void UpdateBoxLength()
    {
        Vector3 containerScale = m_ContainerTransform.localScale;

        // We're assuming the shape of the system is a box, so by
        // scaling the rectangular container, we're hoping to also
        // scale the shape of the system accordingly
        ParticleSystem.ShapeModule shapeModule = m_ParticleSystem.shape;
        shapeModule.scale = containerScale;
    }
}
