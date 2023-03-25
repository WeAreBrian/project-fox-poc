using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tug : MonoBehaviour
{
    private void OnTug()
    {
        FindObjectOfType<Anchor>().Dislodge();
    }
}
