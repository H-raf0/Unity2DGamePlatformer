using UnityEngine;

// This attribute ensures the GameObject will always have these components,
// preventing NullReferenceExceptions if you forget to add them.
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class CharacterScript : MonoBehaviour
{
    #region Components & References
    [Header("Components & References")]
    [Tooltip("Animator component for character animations.")]
    [SerializeField] private Animator myAnimator;
    [Tooltip("Rigidbody2D component for physics.")]
    [SerializeField] private Rigidbody2D rb;
    [Tooltip("Transform used to check if the character is on the ground.")]
    [SerializeField] private Transform groundCheck;
    [Tooltip("LayerMask defining what is considered 'ground'.")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Reference to the child object containing the character's sprite for flipping and rotation.")]
    [SerializeField] private Transform characterSprite;
    #endregion

    #region Audio
    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpSoundClip;
    [SerializeField] private AudioClip hitSoundClip;
    [SerializeField] private AudioClip startSoundClip;
    #endregion

    #region Character Stats
    [Header("Character Stats")]
    [Tooltip("The vertical force applied when jumping.")]
    [SerializeField] private float jumpPower = 21f;
    [Tooltip("The character's horizontal movement speed.")]
    [SerializeField] private float speed = 5f;
    [Tooltip("How fast the character's sprite rotates upon death.")]
    [SerializeField] private float rotationSpeed = 400f;
    [Tooltip("The horizontal force applied when hitting an obstacle.")]
    [SerializeField] private float pushForce = 6f;
    [Tooltip("The vertical force applied when hitting an obstacle.")]
    [SerializeField] private float pushForceY = 8f;
    [Tooltip("The Y position below which the character is considered dead.")]
    [SerializeField] private float deathYPosition = -11f;
    #endregion

    #region Ground Check Settings
    [Header("Ground Check Settings")]
    [Tooltip("The radius of the circle used for the ground check.")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    #endregion

    #region Moving Platform support
    [Header("Moving Platform support")]
    public Rigidbody2D platformRb;
    public bool isOnMovingPlatform = false;
    #endregion

    // Private state variables
    private float horizontalInput;
    private bool isFacingRight = true;
    private bool isAlive = true;
    private float initGravityScale;

    // Temporary input script reference
    private PlayerControllerScript playerControllerScript;

    

    #region Unity Lifecycle Methods
    // Awake is called before the first frame update. Best for initializing components.
    private void Awake()
    {
        if (myAnimator == null) myAnimator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // We also get the player controller script here for robustness.
        playerControllerScript = GetComponent<PlayerControllerScript>();
        if (playerControllerScript == null)
        {
            Debug.LogWarning("PlayerControllerScript not found on the character. Input from that script will not work.");
        }
    }

    private void Start()
    {
        gameObject.name = "Main Character";
        initGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (!isAlive)
        {
            characterSprite.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            return;
        }

        // --- TEMPORARY INPUT HANDLING ---
        // Gathers input from keyboard keys and the external PlayerControllerScript.
        if (Input.GetKey(KeyCode.D) || (playerControllerScript != null && playerControllerScript.moveRight))
        {
            horizontalInput = 1;
        }
        else if (Input.GetKey(KeyCode.A) || (playerControllerScript != null && playerControllerScript.moveLeft))
        {
            horizontalInput = -1;
        }
        else
        {
            horizontalInput = 0;
        }

        if ((Input.GetButtonDown("Jump") || (playerControllerScript != null && playerControllerScript.jump)) && IsGrounded())
        {
            Jump();
        }
        // --- END TEMPORARY INPUT ---

        if (transform.position.y <= deathYPosition)
        {
            Die(false); // Die from falling, no knockback needed.
        }

        Flip();
    }

    // FixedUpdate is the best place for physics calculations.
    private void FixedUpdate()
    {
        if (!isAlive) return;

        // Start with the player's intended velocity based on input and current gravity.
        Vector2 playerVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        // If on a moving platform, add its velocity to the player.
        if (isOnMovingPlatform && platformRb != null)
        {
            // Add the platform's horizontal and vertical velocity.
            // This makes the player move left/right and up/down with the platform.
            Vector2 v = platformRb.linearVelocity;
            v.y = v.y > 0 ? v.y * -1.0f : 0; // Invert vertical velocity if moving up.
            playerVelocity += v;

        }

        rb.linearVelocity = playerVelocity;

        myAnimator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
    }
    #endregion

    #region Character Actions
    private void Jump()
    {
        float verticalVelocity = rb.linearVelocity.y;
        if (isOnMovingPlatform && platformRb != null && platformRb.linearVelocity.y>0)
        {
            verticalVelocity += platformRb.linearVelocity.y;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity + jumpPower);

        if (SoundFXManagerScript.instance != null)
        {
            SoundFXManagerScript.instance.PlaySoundFXClip(jumpSoundClip, transform, 1f);
        }
    }

    private void Flip()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            characterSprite.Rotate(0f, 180f, 0f);
        }
    }

    private void Die(bool applyKnockback)
    {
        if (!isAlive) return;

        isAlive = false;
        Debug.Log("Game Over");

        rb.gravityScale = initGravityScale;
        GetComponent<Collider2D>().enabled = false;

        if (SoundFXManagerScript.instance != null)
        {
            SoundFXManagerScript.instance.PlaySoundFXClip(hitSoundClip, transform, 1f);
        }

        if (applyKnockback)
        {
            float knockbackDirection = isFacingRight ? -1 : 1;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(knockbackDirection * pushForce, pushForceY), ForceMode2D.Impulse);
        }

        if (LevelManagerScript.instance != null)
        {
            LevelManagerScript.instance.GameOver();
        }
    }
    #endregion

    #region Public Methods
    // This new method will be called by the LevelManager to reset the player's state.
    public void ResetState()
    {
        isAlive = true;
        GetComponent<Collider2D>().enabled = true; // Re-enable the collider
        rb.gravityScale = initGravityScale;
        rb.linearVelocity = Vector2.zero; // Stop any residual movement
        characterSprite.rotation = Quaternion.identity; // Reset the sprite's rotation
        isFacingRight = true;

        // play a sound for respawning
        if (SoundFXManagerScript.instance != null)
        {
            SoundFXManagerScript.instance.PlaySoundFXClip(startSoundClip, transform, 1f);
        }
    }
    #endregion

    #region Collision & Ground Check
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<DamageZoneScript>() != null)
        {
            Die(true);
            return;
        }

        // Check the main object for the "Platform" tag.
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnMovingPlatform = true;
            platformRb = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnMovingPlatform = false;
            platformRb = null;
        }
    }
    #endregion
}