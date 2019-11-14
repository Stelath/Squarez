using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Color playerColor;
    public int playerNumber = 1;

    public float speed = 20f;
    public float jumpForce = 20f;

    private float moveInput;
    private Rigidbody2D rb;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius = 0.5f;
    public LayerMask whatIsGround;

    public int extraJumpsValue;
    public float timeInBetweenJumps = 0.5f;
    private int extraJumps = 1;
    private float timeOfLastJump = 0f;

    public float playerHealth = 100f;
    public ParticleSystem deathEffect;

    private void Start()
    {
        // Set the players color
        GetComponent<SpriteRenderer>().color = playerColor;

        // Setup Movement Mechanics
        rb = GetComponent<Rigidbody2D>();
        extraJumps = extraJumpsValue;
    }

    private void FixedUpdate()
    {
        handleMovement();
    }

    private void Update()
    {
        handleJumping();
    }

    private void handleMovement()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        moveInput = Input.GetAxis("P" + playerNumber + "Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    private void handleJumping()
    {
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if ((Input.GetAxisRaw("P" + playerNumber + "Vertical") > 0) && extraJumps > 0 && ((timeOfLastJump + timeInBetweenJumps) <= Time.time))
        {
            rb.velocity = Vector2.up * jumpForce;
            timeOfLastJump = Time.time;
            extraJumps--;
        }
        else if ((Input.GetAxisRaw("P" + playerNumber + "Vertical") > 0) && (extraJumps == 0) && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
            timeOfLastJump = Time.time;
        }
    }

    public void removeHealth(float amountToBeRemoved)
    {
        playerHealth = playerHealth - amountToBeRemoved;
        if(playerHealth <= 0f)
        {
            playerDeath();
        }
    }

    public void playerDeath()
    {
        var targets = Camera.main.GetComponent<CameraController>().targets;
        System.Collections.Generic.List<Transform> listOfTargets = new System.Collections.Generic.List<Transform>(targets);
        listOfTargets.Remove(gameObject.transform);
        Camera.main.GetComponent<CameraController>().targets = listOfTargets.ToArray();

        // Show Player Death Effect
        deathEffect.startColor = playerColor;
        Instantiate(deathEffect, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(deathEffect, 2f);

        // Destroy the Player
        Destroy(gameObject);
    }
}
