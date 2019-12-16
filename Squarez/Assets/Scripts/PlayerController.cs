using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Color playerColor;
    public int playerNumber = 1;

    public bool disabled = false;

    public float speed = 20f;
    public float jumpForce = 20f;

    private float moveInput;
    private Rigidbody2D rb;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask whatIsGround;

    public int extraJumpsValue;
    public float timeInBetweenJumps = 0.4f;
    private int extraJumps = 1;
    private float timeOfLastJump;

    public GameObject objectInHand;
    public float reachRadius = 2;
    private float handRotationInputX;
    private float handRotationInputY;

    public float playerHealth = 100f;
    private bool healthBarShown = false;
    public GameObject healthBarCanvas;
    public GameObject healthBar;

    public ParticleSystem deathEffect;
    public ParticleSystem jumpEffect;

    private void Start()
    {
        // Set the players color
        GetComponent<SpriteRenderer>().color = playerColor;

        // Setup Movement Mechanics
        rb = GetComponent<Rigidbody2D>();
        extraJumps = extraJumpsValue;

        healthBarCanvas.GetComponent<CanvasGroup>().alpha = 0;
    }

    private void FixedUpdate()
    {
        if (!disabled)
        {
            HandleMovement();
        }
    }

    private void Update()
    {
        if (!disabled)
        {
            HandleJumping();
            HandleHandRotation();
            HandleHandItem();
        }
    }

    private void HandleMovement()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        moveInput = Input.GetAxis("P" + playerNumber + "Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    private void HandleJumping()
    {
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if (((-Input.GetAxis("P" + playerNumber + "Vertical") > 0.8) || (Input.GetAxisRaw("P" + playerNumber + "VerticalButton") > 0)) && extraJumps > 0 && ((timeOfLastJump + timeInBetweenJumps) <= Time.time))
        {
            rb.velocity = Vector2.up * jumpForce;
            timeOfLastJump = Time.time;
            extraJumps--;
            PlayJumpEffect();
        }
        else if (((-Input.GetAxis("P" + playerNumber + "Vertical") > 0.8) || (Input.GetAxisRaw("P" + playerNumber + "VerticalButton") > 0)) && (extraJumps == 0) && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
            timeOfLastJump = Time.time;
            PlayJumpEffect();
        }
    }

    public void PlayJumpEffect()
    {
        jumpEffect.startColor = playerColor;
        ParticleSystem instantiatedJumpEffect = Instantiate(jumpEffect, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(instantiatedJumpEffect.gameObject, 2f);
    }

    public void HandleHandRotation()
    {
        handRotationInputX = Input.GetAxis("P" + playerNumber + "HandHorizontal");
        handRotationInputY = -(Input.GetAxis("P" + playerNumber + "HandVertical"));
        if (objectInHand != null)
        {
            if (handRotationInputX == 0f && handRotationInputY == 0f)
            {
                Vector3 curRot = gameObject.transform.localEulerAngles;
                Vector3 homeRot;
                if (curRot.z > 180f)
                {
                    homeRot = new Vector3(0f, 0f, 359.999f);
                }
                else
                {
                    homeRot = Vector3.zero;
                }
                objectInHand.transform.localEulerAngles = Vector3.Slerp(curRot, homeRot, Time.deltaTime * 2);
            }
            else
            {
                objectInHand.transform.localEulerAngles = new Vector3(0f, 0f, (Mathf.Atan2(handRotationInputY, handRotationInputX) * 180 / Mathf.PI) - transform.eulerAngles.z); // this does the actual rotaion according to inputs
            }
        }
    }

    public void HandleHandItem()
    {
        if (objectInHand != null && (Input.GetAxisRaw("P" + playerNumber + "Drop") > 0))
        {
            var droppedGun = Instantiate(objectInHand, objectInHand.transform.position, objectInHand.transform.rotation);
            droppedGun.AddComponent<Rigidbody2D>();
            droppedGun.AddComponent<PolygonCollider2D>();
            droppedGun.GetComponent<GunController>().canFire = false;
            droppedGun.GetComponent<Rigidbody2D>().velocity = 5 * new Vector2(objectInHand.transform.right.x, objectInHand.transform.right.y);
            Destroy(objectInHand);
        }

        if (objectInHand == null && (Input.GetAxisRaw("P" + playerNumber + "Grab") > 0))
        {
            Collider2D[] objectsNearPlayer = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), reachRadius);
            GameObject closestObject = null;
            var closestObjectDistance = reachRadius + 1f;

            foreach (var grabbableObject in objectsNearPlayer)
            {
                var distanceToObject = Vector3.Distance(gameObject.transform.position, grabbableObject.gameObject.transform.position);
                if ((grabbableObject.gameObject.GetComponent<GunController>() != null) && (distanceToObject < closestObjectDistance))
                {
                    closestObject = grabbableObject.gameObject;
                    closestObjectDistance = distanceToObject;
                }
            }

            if (closestObject != null)
            {
                objectInHand = Instantiate(closestObject, gameObject.transform);
                objectInHand.transform.position = transform.position;
                objectInHand.GetComponent<GunController>().canFire = true;
                objectInHand.GetComponent<GunController>().playerNumber = playerNumber;
                objectInHand.GetComponent<GunController>().player = gameObject;

                Destroy(objectInHand.GetComponent<Rigidbody2D>());
                Destroy(objectInHand.GetComponent<PolygonCollider2D>());
                Destroy(closestObject);
            }
        }
    }

    public void RemoveHealth(float amountToBeRemoved)
    {
        playerHealth = playerHealth - amountToBeRemoved;

        if(playerHealth <= 0f)
        {
            PlayerDeath();
        }

        StartCoroutine(FlashWhite());

        if (healthBarShown)
        {
            StopCoroutine("UpdateHelathBar");
        }

        StartCoroutine("UpdateHelathBar");
    }

    IEnumerator FlashWhite()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(playerColor.r + 10, playerColor.g + 10, playerColor.b + 10, 1f);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = playerColor;
    }

    IEnumerator UpdateHelathBar()
    {
        if (!healthBarShown)
        {
            StopCoroutine("DecreaseHealthBar");
            StopCoroutine("FadeOutHealthBar");
            StartCoroutine("FadeInHealthBar");
            yield return new WaitForSeconds(0.2f);
        }
        
        StartCoroutine("DecreaseHelathBar");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FadeOutHealthBar");
    }

    IEnumerator FadeInHealthBar()
    {
        healthBarShown = true;
        for(float f = 0.05f; f <= 1f; f += 0.05f)
        {
            healthBarCanvas.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator FadeOutHealthBar()
    {
        healthBarShown = false;
        for (float f = 1f; f >= -0.5; f -= 0.05f)
        {
            healthBarCanvas.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator DecreaseHelathBar()
    {
        var healthBarSlider = healthBar.GetComponent<Slider>();
        var waitTime = 0.15f / ((healthBarSlider.value - (playerHealth / 100)) / 0.05f) ;

        for (float i = healthBarSlider.value; i >= (playerHealth/100) - 0.05; i -= 0.05f)
        {
            healthBarSlider.value = i;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void PlayerDeath()
    {

        var targets = Camera.main.GetComponent<CameraController>().targets;
        System.Collections.Generic.List<Transform> listOfTargets = new System.Collections.Generic.List<Transform>(targets);
        listOfTargets.Remove(gameObject.transform);
        Camera.main.GetComponent<CameraController>().targets = listOfTargets.ToArray();

        // Show Player Death Effect
        deathEffect.startColor = playerColor;
        ParticleSystem instantiatedDeathEffect = Instantiate(deathEffect, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(instantiatedDeathEffect.gameObject, 2f);

        var gameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        var livingPlayers = new GameObject[gameControllerScript.players.Length - 1];
        var i = 0;
        foreach (var player in gameControllerScript.players)
        {
            if (player != gameObject)
            {
                livingPlayers[i] = player;
                i++;
            }
        }
        gameControllerScript.players = livingPlayers;

        // Destroy the Player
        Destroy(gameObject);
    }
}
