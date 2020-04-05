using UnityEngine;

public class LavaController : MonoBehaviour
{
    public ParticleSystem lavaSplash;

    void OnCollisionEnter2D(Collision2D other)
    {
        var objectCollidedWith = other.collider;
        if (objectCollidedWith.GetComponent<PlayerController>())
        {
            var playerController = objectCollidedWith.GetComponent<PlayerController>();
            var instLavaSplash = Instantiate(lavaSplash, objectCollidedWith.transform.position, new Quaternion(0, 0, 0, 0));
            Destroy(instLavaSplash.gameObject, 2f);
            playerController.PlayerDeath(true);
        }
        else if (objectCollidedWith.GetComponent<GunController>())
        {
            var gunController = objectCollidedWith.GetComponent<GunController>();
            var instLavaSplash = Instantiate(lavaSplash, objectCollidedWith.transform.position, new Quaternion(0, 0, 0, 0));
            Destroy(instLavaSplash.gameObject, 2f);
            Destroy(gunController);
        }
    }
}
