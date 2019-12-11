using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletDamage;
    public float bulletKnockback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            var player = collision.GetComponent<PlayerController>();
            player.RemoveHealth(bulletDamage);

            player.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity + (bulletKnockback * new Vector2(gameObject.transform.forward.x, gameObject.transform.forward.y));

        }

        Destroy(gameObject);
    }
}
