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
    public float checkRadius = 1;
    public LayerMask whatIsGround;

    public int extraJumpsValue;
    public float timeInBetweenJumps = 0.5f;
    private int extraJumps = 1;
    private float timeOfLastJump;

    public float playerHealth = 100f;

    public ParticleSystem deathEffect;
    public ParticleSystem jumpEffect;

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

        if ((Input.GetAxisRaw("P" + playerNumber + "Jump") > 0) && extraJumps > 0 && ((timeOfLastJump + timeInBetweenJumps) <= Time.time))
        {
            rb.velocity = Vector2.up * jumpForce;
            timeOfLastJump = Time.time;
            extraJumps--;
            playJumpEffect();
        }
        else if ((Input.GetAxisRaw("P" + playerNumber + "Jump") > 0) && (extraJumps == 0) && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
            timeOfLastJump = Time.time;
            playJumpEffect();
        }
    }

    public void playJumpEffect()
    {
        jumpEffect.startColor = playerColor;
        ParticleSystem instantiatedJumpEffect = Instantiate(jumpEffect, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(instantiatedJumpEffect.gameObject, 2f);
    }

    public void RemoveHealth(float amountToBeRemoved)
    {
        playerHealth = playerHealth - amountToBeRemoved;
        if(playerHealth <= 0f)
        {
            PlayerDeath();
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

        // Destroy the Player
        Destroy(gameObject);
    }
}
