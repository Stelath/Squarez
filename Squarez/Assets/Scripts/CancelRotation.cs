using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelRotation : MonoBehaviour
{
    public GameObject parent;
    public Vector3 offset;

    void Update()
    {
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        transform.position = parent.transform.position + offset;
    }
}
