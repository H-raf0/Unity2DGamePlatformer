using UnityEngine;

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
    [Tooltip("A small cooldown to prevent jump spamming and glitches.")]
    [SerializeField] private float jumpCooldown = 0.1f;
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
    // We go back to this method of tracking the platform
    private Rigidbody2D platformRb;
    private bool isOnMovingPlatform = false;
    #endregion

    // Private state variables
    private float horizontalInput;
    private bool isFacingRight = true;
    private bool isAlive = true;
    private float initGravityScale;
    private float lastJumpTime = -1f;

    private PlayerControllerScript playerControllerScript;

    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (myAnimator == null) myAnimator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        playerControllerScript = GetComponent<PlayerControllerScript>();
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

        // --- Input Handling ---
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

        if ((Input.GetButtonDown("Jump") || (playerControllerScript != null && playerControllerScript.jump)))
        {
            if (Time.time >= lastJumpTime + jumpCooldown && IsGrounded())
            {
                Jump();
            }
        }

        if (transform.position.y <= deathYPosition)
        {
            Die(false);
        }

        Flip();
    }

    // --- REVISED FIXEDUPDATE ---
    private void FixedUpdate()
    {
        if (!isAlive) return;

        // Calculate the player's intended velocity based on input.
        Vector2 playerVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        // If on a moving platform, add the platform's velocity.
        // This makes the player "stick" to the platform and inherit its movement naturally.
        if (isOnMovingPlatform && platformRb != null)
        {
            playerVelocity.x += platformRb.linearVelocity.x;
        }

        // 3. Apply the final calculated velocity.
        rb.linearVelocity = playerVelocity;

        myAnimator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
    }
    #endregion

    #region Character Actions

    // --- REVISED JUMP ---
    private void Jump()
    {
        if (isOnMovingPlatform) rb.gravityScale = initGravityScale;
        lastJumpTime = Time.time;

        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

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

        isOnMovingPlatform = false;
        platformRb = null;

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
    public void ResetState()
    {
        isAlive = true;
        GetComponent<Collider2D>().enabled = true;
        rb.gravityScale = initGravityScale;
        rb.linearVelocity = Vector2.zero;
        characterSprite.rotation = Quaternion.identity;
        isFacingRight = true;

        isOnMovingPlatform = false;
        platformRb = null;

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

        if (collision.gameObject.CompareTag("Platform"))
        {
            // By checking the contact points, we ensure we only attach to the platform when landing ON TOP of it.
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // The collision is from below the player
                {
                    isOnMovingPlatform = true;
                    rb.gravityScale = 20f; // Temporarily increase gravity to ensure the player sticks to the platform
                    platformRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            rb.gravityScale = initGravityScale;
            isOnMovingPlatform = false;
            platformRb = null;
        }
    }
    #endregion
}