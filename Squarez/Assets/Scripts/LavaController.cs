using UnityEngine;

public class LavaController : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        var objectCollidedWith = other.collider;
        if (objectCollidedWith.GetComponent<PlayerController>())
        {
            var playerController = objectCollidedWith.GetComponent<PlayerController>();
            playerController.PlayerDeath();
        }
    }
}
