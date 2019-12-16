using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject player;

    public GameObject bullet;
    public Transform muzzle;
    public int playerNumber = 1;

    public float bulletDamage = 20f;
    public float bulletSpread = 0.03f;
    public float amountOfBullets = 1f;
    public float firingForce = 30f;
    public float fireRate = 0.5f;
    public bool canFire = true;

    private float lastTimeFired;

    public float muzzleFlashFPS = 30.0f;
    public Texture2D[] frames;
    public GameObject muzzleFlash;

    private int frameIndex;
    private MeshRenderer muzzleFlashMeshRenderer;

    private void Start()
    {
        muzzleFlashMeshRenderer = muzzleFlash.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
    }

    private void Fire()
    {
        if ((Input.GetAxis("P" + playerNumber + "Fire") > 0) && (fireRate <= (Time.time - lastTimeFired)) && canFire)
        {
            for (var i = 0; i < amountOfBullets; i++)
            {
                var bulletFired = Instantiate(bullet, muzzle.position, muzzle.rotation);
                bulletFired.GetComponent<BulletController>().bulletDamage = bulletDamage;
                bulletFired.GetComponent<Rigidbody2D>().velocity = firingForce * new Vector2(muzzle.right.x, muzzle.right.y - Random.Range(-bulletSpread, bulletSpread));

                Destroy(bulletFired, 10f);
            }

            lastTimeFired = Time.time;
            PlayMuzzleFlash();
        }
    }

    private void PlayMuzzleFlash()
    {
        InvokeRepeating("NextMuzzleFlashFrame", 0f, (1 / muzzleFlashFPS));
        Invoke("CancelMuzzleFlashInvoke", (1/muzzleFlashFPS) * frames.Length);
    }

    private void CancelMuzzleFlashInvoke()
    {
        CancelInvoke("NextMuzzleFlashFrame");
    }

    private void NextMuzzleFlashFrame()
    {
        muzzleFlashMeshRenderer.material.SetTexture("_MainTex", frames[frameIndex]);
        frameIndex = (frameIndex + 1) % frames.Length;
    }
}
