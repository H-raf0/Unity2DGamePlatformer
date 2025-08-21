using UnityEngine;
using System.Collections; 

public class MovingPlatformScript : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Waypoints")]
    public Transform pointA;
    public Transform pointB;

    [Header("Splitting")]
    public bool canSplit = false;
    [Tooltip("How fast the pieces move apart.")]
    public float splitSpeed = 3f; // Changed from splitDuration
    [Tooltip("How far each piece moves outward from its starting position.")]
    public float splitDistance = 1f;


    private Transform leftChild;
    private Transform rightChild;

    private Vector3 currentTarget;

    private Rigidbody2D rb;
    private Vector3 moveDirection;

    private bool hasSplit = false; // Prevents the split logic from running multiple times


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (transform.childCount == 2)
        {
            if (transform.GetChild(0).localPosition.x < transform.GetChild(1).localPosition.x)
            {
                leftChild = transform.GetChild(0);
                rightChild = transform.GetChild(1);
            }
            else
            {
                leftChild = transform.GetChild(1);
                rightChild = transform.GetChild(0);
            }
        }
    }


    private void Start()
    {
        // Start by moving towards Point B
        currentTarget = pointB.position;
        CalculateDirection();
    }

    private void Update()
    {

        // Check if the platform has reached the current target
        if (Vector3.Distance(transform.position, currentTarget) < 0.1f)
        {
            // If the current target was Point B, switch to Point A
            if (currentTarget == pointB.position)
            {
                currentTarget = pointA.position;
            }
            // If the current target was Point A, switch to Point B
            else
            {
                currentTarget = pointB.position;
            }
            CalculateDirection();
        }
    }

    private void FixedUpdate()
    {

        rb.linearVelocity = moveDirection * speed;
    }

    private void CalculateDirection()
    {
        // Calculate the direction to move towards the current target
        moveDirection = (currentTarget - transform.position).normalized;
    }

    private void Split()
    {
        // Only run this logic if the platform can split and hasn't split already
        if (canSplit && !hasSplit)
        {
            hasSplit = true;
            Debug.Log("Platform is splitting!");

            // Activate the colliders on the child objects
            Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in childColliders)
            {
                col.enabled = true;
            }

            // Deactivate all colliders on THIS parent object
            Collider2D[] parentColliders = GetComponents<Collider2D>();
            foreach (Collider2D col in parentColliders)
            {
                col.enabled = false;
            }

            StartCoroutine(OpenPlatformRoutine());
        }
    }


    // This coroutine animates the children moving apart over time using SPEED.
    private IEnumerator OpenPlatformRoutine()
    {
        // 1. Get the starting and target LOCAL positions of the children
        Vector3 leftTargetPos = leftChild.localPosition + Vector3.left * splitDistance;
        Vector3 rightTargetPos = rightChild.localPosition + Vector3.right * splitDistance;

        // 2. Loop until both children have reached their destination
        while (leftChild.localPosition != leftTargetPos || rightChild.localPosition != rightTargetPos)
        {
            // 3. Move each child towards its target at a constant speed
            leftChild.localPosition = Vector3.MoveTowards(leftChild.localPosition, leftTargetPos, splitSpeed * Time.deltaTime);
            rightChild.localPosition = Vector3.MoveTowards(rightChild.localPosition, rightTargetPos, splitSpeed * Time.deltaTime);

            // Wait for the next frame before continuing the loop
            yield return null;
        }

        Debug.Log("Platform finished opening.");
        // The snapping at the end is no longer necessary, as MoveTowards is very precise.
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Split();
        }
    }
}