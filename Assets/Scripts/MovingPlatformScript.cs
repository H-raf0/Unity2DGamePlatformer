using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Waypoints")]
    public Transform pointA;
    public Transform pointB;

    [Header("splittable")]
    public bool canSplit = false;

    private Vector3 initPointA;
    private Vector3 initPointB;
    private Vector3 currentTarget;

    
    private Rigidbody2D rb;
    private Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {   
        initPointA = pointA.position;
        initPointB = pointB.position;
        // Start by moving towards Point B
        currentTarget = initPointB;
        CalculateDirection();
    }

    private void Update()
    {
        // Check if the platform has reached the current target
        // A very small threshold like 0.01f is used for accuracy.
        if (Vector3.Distance(transform.position, currentTarget) < 0.1f)
        {
            // If the current target was Point B, switch to Point A
            if (currentTarget == initPointB)
            {
                currentTarget = initPointA;
                CalculateDirection();
            }
            // If the current target was Point A, switch to Point B
            else
            {
                currentTarget = initPointB;
                CalculateDirection();
            }
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
    


}