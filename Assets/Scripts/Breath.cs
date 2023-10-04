using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Breath : MonoBehaviour
{
    [SerializeField]
    private Image m_BreathUI;
    [SerializeField]
    private float m_BreathMax;
    private float m_BreathAmount;

    [SerializeField]
    private AudioClip m_SuffocateSound;

    private bool m_Submerged;


    private CloseOrOpenCircle m_HoleTransition;

    // Start is called before the first frame update
    void Start()
    {
        m_BreathAmount = m_BreathMax;
        var color = m_BreathUI.color;
        color.a = 0;
        m_BreathUI.color = color;


        if (GameObject.Find("HoleTransition") == null)
        {
            Debug.Log("Can't find the LevelTransitioner prefab in the scene. Ask Sach if help is needed.");
        }
        else
        {
            m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Submerged)
        {
            if (m_BreathAmount > 0 && m_BreathAmount - Time.deltaTime <= 0)
            {
                AudioController.PlaySound(m_SuffocateSound, 0.6f, 1, MixerGroup.SFX)
                    ;
                if (m_HoleTransition != null)
                {
                    StartCoroutine(m_HoleTransition.ShrinkParentObject(SceneManager.GetActiveScene().buildIndex));
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            m_BreathAmount -= Time.deltaTime;
        }
        else
        {
            if (m_BreathAmount < m_BreathMax && m_BreathAmount+Time.deltaTime >= m_BreathMax)
            {
                LeanTween.value(gameObject, 255, 0, 0.3f).setOnUpdate((float val) =>
                {
                    Color c = m_BreathUI.color;
                    c.a = val;
                    m_BreathUI.color = c;
                });
            }
            m_BreathAmount = Mathf.Min(m_BreathAmount+Time.deltaTime, m_BreathMax);
            
        }
        m_BreathUI.fillAmount = m_BreathAmount / m_BreathMax;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            m_Submerged = true;
            LeanTween.value(gameObject, 0, 255, 0.5f).setOnUpdate((float val) =>
            {
                Color c = m_BreathUI.color;
                c.a = val;
                m_BreathUI.color = c;
            });
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            m_Submerged = false;
            LeanTween.value(gameObject, 255, 0, 0.3f).setOnUpdate((float val) =>
            {
                Color c = m_BreathUI.color;
                c.a = val;
                m_BreathUI.color = c;
            });
            LeanTween.value(gameObject, m_BreathAmount, m_BreathMax, 0.3f).setOnUpdate((float val) =>
            {
                m_BreathAmount = val;
            });
        }
    }
}
