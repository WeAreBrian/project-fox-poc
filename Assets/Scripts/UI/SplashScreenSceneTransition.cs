using UnityEngine;

public class SplashScreenSceneTransition : MonoBehaviour
{
    private CloseOrOpenCircle m_HoleTransition;

    private void Awake()
    {
        m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            StartCoroutine(m_HoleTransition.ShrinkParentObject(2));
        }
    }
}
