using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    private Vector3 dir;
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GameObject.Find("UI").transform);
        StartCoroutine(Finish(lifetime));
    }

    // Update is called once per frame
    void Update()
    {

        var x = Mathf.Sin(Time.time * 0.1f) * 1f;

        transform.position = new Vector3(startingPos.x+x, transform.position.y+(dir.y/1000), 0);
    }

    public void Set(string value, Vector2 position, Color colour)
    {
        text.text = value;
        startingPos = position;
        text.color = colour;
    }

    public IEnumerator Finish(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
