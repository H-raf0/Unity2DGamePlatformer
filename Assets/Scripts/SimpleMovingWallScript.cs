using UnityEngine;
using System.Collections;

public class SimpleMovingWallScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Target coordinates the wall will move to when triggered.")]
    [SerializeField] private Vector3 targetCoordinates = Vector3.zero;
    [Tooltip("How fast the wall moves.")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("How long to wait at the target position before returning.")]
    [SerializeField] private float waitTime = 3f;

    [Header("Trigger Settings")]
    [Tooltip("What layer or tag can trigger this wall.")]
    [SerializeField] private bool triggerOnPlayerEnter = true;

    private Vector3 originalPosition;
    private bool isMoving = false;
    private bool hasTriggered = false;

    void Awake()
    {
        // Store the starting position
        originalPosition = transform.position;
    }

    // Trigger methods for different use cases
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnPlayerEnter && other.CompareTag("Player") && !hasTriggered)
        {
            TriggerMovement();
        }
    }

    // Public method to trigger movement from other scripts
    public void TriggerMovement()
    {
        if (!isMoving && !hasTriggered)
        {
            StartCoroutine(MoveSequence());
        }
    }

    // Reset method to allow retriggering (optional)
    public void ResetWall()
    {
        if (!isMoving)
        {
            hasTriggered = false;
            transform.position = originalPosition;
        }
    }

    private IEnumerator MoveSequence()
    {
        isMoving = true;
        hasTriggered = true;

        Debug.Log($"{gameObject.name} starting movement sequence");

        // Move to target coordinates
        while (Vector3.Distance(transform.position, targetCoordinates) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetCoordinates, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure exact positioning
        transform.position = targetCoordinates;
        Debug.Log($"{gameObject.name} reached target position");

        // Wait at target position
        yield return new WaitForSeconds(waitTime);

        // Return to original position
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure exact positioning
        transform.position = originalPosition;
        Debug.Log($"{gameObject.name} returned to original position");

        isMoving = false;
    }
}