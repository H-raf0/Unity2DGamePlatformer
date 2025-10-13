using UnityEngine;
using System.Collections; // Needed for Coroutines

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
    [Tooltip("enable or disable the jumping cut")]
    [SerializeField] private bool jumpCutEnabled = false;
    [Tooltip("How much to reduce upward velocity when the jump button is released early.")]
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [Tooltip("How fast the character's sprite rotates upon death.")]
    [SerializeField] private float rotationSpeed = 400f;
    [Tooltip("The horizontal force applied when hitting an obstacle.")]
    [SerializeField] private float pushForceX = 6f;
    [Tooltip("The vertical force applied when hitting an obstacle.")]
    [SerializeField] private float pushForceY = 8f;
    [Tooltip("The Y position below which the character is considered dead.")]
    [SerializeField] private float deathYPosition = -11f;
    [Tooltip("The default gravity scale for the player.")]
    [SerializeField] private float defaultGravityScale = 3f; // New: Explicit default gravity
    #endregion

    #region Ground Check Settings
    [Header("Ground Check Settings")]
    [Tooltip("The radius of the circle used for the ground check.")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    #endregion

    #region Moving Platform support
    private Rigidbody2D platformRb;
    private bool isOnMovingPlatform = false;
    #endregion

    // Private state variables
    private float horizontalInput;
    private float lastJumpTime = -1f;
    private bool isFacingRight = true;
    private bool isAlive = true;
    private float knockbackEndTime = 0f;
    private bool isKnockedBack = false;
    private bool wasJumpingLastFrame = false; // Track jump state for mobile

    private PlayerControllerScript playerControllerScript;

    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (myAnimator == null) myAnimator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        playerControllerScript = GetComponent<PlayerControllerScript>();

        // Ensure default gravity is set if it's not set in the inspector.
        // This is a more robust way to initialize than relying on Start, which might run later.
        if (rb.gravityScale <= 0.01f) // Check for near-zero to catch accidental small values
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    private void Start()
    {
        gameObject.name = "Main Character";
        // initGravityScale is now `defaultGravityScale` if not explicitly set in Awake.
        // Ensure gravity is active from the start, using the robust default.
        rb.gravityScale = defaultGravityScale;
    }

    private void Update()
    {
        // Handle knockback rotation here in Update as it's visual.
        if (!isAlive && isKnockedBack) // Only rotate if dead AND knocked back
        {
            characterSprite.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        if (isKnockedBack && Time.time < knockbackEndTime)
        {
            return; // Block other actions while knocked back
        }
        else if (isKnockedBack && Time.time >= knockbackEndTime)
        {
            // Knockback duration ended
            isKnockedBack = false;
            rb.linearVelocity = Vector2.zero; // Stop any residual knockback velocity
            rb.gravityScale = defaultGravityScale; // Ensure gravity is back to normal
        }


        if (!isAlive)
        {
            return;
        }

        // --- Input Handling ---
        HandleInput();

        // --- Jump Cut Logic (Fixed for Mobile) ---
        HandleJumpCut();

        if (transform.position.y <= deathYPosition)
        {
            Die(false); // Die without knockback if falling off the map
        }

        Flip();
    }

    private void HandleInput()
    {
        // Horizontal Input
        // Prioritize PC input if available, otherwise use mobile input.
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1;
        }
        else if (playerControllerScript != null && playerControllerScript.moveRight)
        {
            horizontalInput = 1;
        }
        else if (playerControllerScript != null && playerControllerScript.moveLeft)
        {
            horizontalInput = -1;
        }
        else
        {
            horizontalInput = 0;
        }

        // Jump Input
        bool jumpPressedThisFrame = Input.GetButtonDown("Jump") ||
                                    (playerControllerScript != null && playerControllerScript.jump && !wasJumpingLastFrame);

        if (jumpPressedThisFrame)
        {
            if (Time.time >= lastJumpTime + jumpCooldown && IsGrounded())
            {
                Jump();
            }
        }

        // Update jump state tracking for mobile (important for jump cut and single-tap detection)
        wasJumpingLastFrame = playerControllerScript != null && playerControllerScript.jump;
    }

    private void HandleJumpCut()
    {
        if (!jumpCutEnabled) return;

        bool jumpReleasedThisFrame = Input.GetButtonUp("Jump") ||
                                     (playerControllerScript != null && wasJumpingLastFrame && !playerControllerScript.jump);

        if (jumpReleasedThisFrame && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack || !isAlive) return; // Block movement if knocked back or dead

        // Calculate the player's intended velocity based on input.
        Vector2 playerVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        // Move with platform
        if (isOnMovingPlatform && platformRb != null)
        {
            // Only add platform velocity if the player is actively moving horizontally or standing still.
            // This prevents the platform from dragging the player too much when they're trying to move against it.
            playerVelocity.x += platformRb.linearVelocity.x;
        }

        // Apply the final calculated velocity.
        rb.linearVelocity = playerVelocity;

        myAnimator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
    }
    #endregion

    #region Character Actions

    private void Jump()
    {
        lastJumpTime = Time.time;

        // Ensure gravity is reset to default when jumping off a platform
        // The high gravity on platforms might interfere with jump physics.
        rb.gravityScale = defaultGravityScale;

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

        // Reset platform states
        isOnMovingPlatform = false;
        platformRb = null;

        // Disable collider immediately to prevent further collision triggers
        GetComponent<Collider2D>().enabled = false;

        // Apply knockback if specified
        if (applyKnockback)
        {
            rb.linearVelocity = Vector2.zero; // Clear current velocity before applying force
            float knockbackDirection = isFacingRight ? -1 : 1;
            Vector2 force = new Vector2(knockbackDirection * pushForceX, pushForceY);
            ApplyKnockback(force, 20f); // Use knockbackDuration for death knockback
            Debug.Log("Knockback applied on death");
        }
        else
        {
            // If no knockback, just ensure gravity is normal so player falls
            rb.linearVelocity = Vector2.zero; // Stop horizontal movement
            rb.gravityScale = defaultGravityScale; // Ensure falling naturally
        }

        if (SoundFXManagerScript.instance != null)
        {
            SoundFXManagerScript.instance.PlaySoundFXClip(hitSoundClip, transform, 1f);
        }

        if (LevelManagerScript.instance != null)
        {
            LevelManagerScript.instance.GameOver();
        }
    }
    #endregion

    #region Public Methods

    public void ApplyKnockback(Vector2 force, float duration)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + duration;

        rb.linearVelocity = Vector2.zero; // Clear current velocity
        rb.AddForce(force, ForceMode2D.Impulse);

        // Temporarily ensure gravity is active during knockback to prevent floating
        rb.gravityScale = defaultGravityScale;
    }

    public void ResetState()
    {
        isAlive = true;
        GetComponent<Collider2D>().enabled = true;
        rb.gravityScale = defaultGravityScale; // Ensure gravity is always reset to default
        rb.linearVelocity = Vector2.zero;
        characterSprite.rotation = Quaternion.identity;
        isFacingRight = true;
        isKnockedBack = false;
        wasJumpingLastFrame = false;

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
            Die(true); // Die with knockback if hitting a damage zone
            return;
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Only consider it a "moving platform" if we land on top.
                if (contact.normal.y > 0.5f) // Normal pointing up, meaning we landed on it
                {
                    isOnMovingPlatform = true;
                    // Significantly increased gravity to stick player to platform.
                    // This is a common technique, but we MUST ensure it's reset.
                    rb.gravityScale = 20f;
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
            // Crucial: Reset gravity to the default when leaving the platform.
            rb.gravityScale = defaultGravityScale;
            isOnMovingPlatform = false;
            platformRb = null;
        }
    }

    // Visualize the ground check in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    #endregion
}