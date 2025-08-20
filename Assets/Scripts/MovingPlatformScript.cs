using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 targetPosition;
    public float speed = 2f;

    private Vector2 initPosition;
    private bool goingToTarget = true; // Track which way we're moving

    protected virtual void Start()
    {
        initPosition = transform.position;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected void Move()
    {
        Vector2 currentTarget = goingToTarget ? targetPosition : initPosition;
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget) < 0.01f)
        {
            goingToTarget = !goingToTarget; // Flip direction
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.parent = null;
        }
    }
}

