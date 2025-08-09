using UnityEngine;

public class characterScript : MonoBehaviour
{
    [SerializeField] private Animator myAnimator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioClip jumpSoundClip;
    [SerializeField] private AudioClip hitSoundClip;

    public float jumpPower = 21f;
    public float speed = 5f;
    public Transform characterSprite;
    public float rotationSpeed = 400f; // Rotation speed when the character dies
    public float pushForce = 6f;      // force applied to the character in the opposite direction when it hits an obstacle 

    private int horizontalInput;       // -1 for left, 0 for none, 1 for right
    private bool isFacingRight = true;
    private bool isAlive = true;

    void Start()
    {
        gameObject.name = "Main Character";
        //logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    private void Update()
    {
        if (isAlive)
        {
            if (Input.GetKey(KeyCode.D))
            {
                horizontalInput = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                horizontalInput = -1;
            }
            else
            {
                horizontalInput = 0;
            }

            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                SoundFXManager.instance.PlaySoundFXClip(jumpSoundClip, transform, 1f);
            }

            if (rb.position.y <= -11f)
            {
                isAlive = false;
                LogicScript.instance.GameOver();
                Debug.Log("Game Over");
            }

            Flip();
        }
        else
        {
            // Rotate the character when it is not alive
            characterSprite.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            // Move character horizontally based on input
            float targetSpeed = horizontalInput * speed;

            // Apply the horizontal velocity (no change to Y velocity)
            rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);

            myAnimator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that triggered the event is the player
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            //Debug.Log("Player hit an obstacle!");
            isAlive = false;
            SoundFXManager.instance.PlaySoundFXClip(hitSoundClip, transform, 1f);

            // Push the character when it is not alive in an opposite direction
            // should I include it in the game over function? no! 
            float yVelocitySign = Mathf.Sign(rb.linearVelocity.y);
            rb.linearVelocity = new Vector2((isFacingRight ? -1 : 1) * pushForce, pushForce * 2.5f);

            LogicScript.instance.GameOver();
        }
    }
}
