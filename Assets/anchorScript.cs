 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anchorScript : MonoBehaviour
{
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }


}
