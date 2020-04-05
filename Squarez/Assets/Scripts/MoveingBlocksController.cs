using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveingBlocksController : MonoBehaviour
{
    public float direction = 0f;
    public float speed = 0f;

    void Update()
    {
        if (direction == 0)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * 1, 0);
        }
        else if (direction == 1)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed * 1);
        }
        else if (direction == 2)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed * 1, 0);
        }
        else if (direction == 3)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -speed * 1);
        }
    }
}
