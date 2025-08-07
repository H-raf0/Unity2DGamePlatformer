using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Speed of movement
    private bool hasMoved = false; // Prevent multiple triggers
    private float multiplier = 1f; // Direction multiplier
    private Vector2 direction;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasMoved)
        {
            hasMoved = true;

            // Get all colliders attached to the GameObject
            Collider2D[] colliders = GetComponents<Collider2D>();

            // Find the specific collider you want (e.g., CircleCollider2D)
            CircleCollider2D circleCollider = null;

            foreach (var collider in colliders)
            {
                if (collider is CircleCollider2D)
                {
                    circleCollider = (CircleCollider2D)collider;
                }
            }

            // Use the specific collider's offset
            if (circleCollider != null)
            {
                float offsetX = circleCollider.offset.x;
                if (offsetX < -1f)
                {
                    Debug.Log("Circle Collider Offset X: " + offsetX);
                    multiplier = 8f;
                    moveSpeed = 2f;
                }
            }

            StartCoroutine(MoveSpike());
        }
    }

    IEnumerator MoveSpike()
    {
        Vector2 startPosition = transform.position;
        float zRotation = transform.eulerAngles.z;
        Debug.Log("ROTATION: " + zRotation);

        if (zRotation == 0 || zRotation == 180)
        {
            direction.y = 0;
            if (zRotation == 180)
            {
                direction.x = 1;
            }
            else
            {
                direction.x = -1;
            }
        }
        else
        {
            direction.x = 0;
            if (zRotation == 90)
            {
                direction.y = -1;
            }
            else if (zRotation == 270)
            {
                direction.y = 1;
            }
        }

        Debug.Log("Direction: " + direction);
        Vector2 targetPosition = startPosition + direction * multiplier; // Move forward

        float time = 0;
        while (time < 1)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time);
            time += Time.deltaTime * moveSpeed;
            yield return null;
        }

        transform.position = targetPosition; // Ensure final position is exact
    }
}
