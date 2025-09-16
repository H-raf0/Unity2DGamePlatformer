using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PushWallScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the wall moves between points.")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Waypoints")]
    [Tooltip("List of Transform points the wall will move to in order.")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    public bool conditionMet = false;

    // Private variables to track the state
    private Vector3 originalPosition;
    private bool isMoving = false; // Flag to prevent the coroutine from starting multiple times
    private bool inPosition = false;

    void Awake()
    {
        // Store the starting position of the wall.
        originalPosition = transform.position;
    }

    void Update()
    {
        // If the player is in position and the wall isn't already moving, start the movement coroutine.
        if (inPosition && !isMoving && conditionMet && waypoints.Count > 0)
        {
            StartCoroutine(MoveWall());
        }
    }

    // This is called ONLY ONCE when the player first enters the trigger.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object is the player AND if the wall isn't already moving.
        if (other.CompareTag("Player") && !isMoving)
        {
            inPosition = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            inPosition = false;
        }
    }

    // This is the coroutine that handles the entire movement sequence.
    private IEnumerator MoveWall()
    {
        // Set the flag so we can't be re-triggered.
        isMoving = true;

        // Move through each waypoint in order
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint == null)
            {
                Debug.LogWarning($"Null waypoint found in {gameObject.name}. Skipping.");
                continue;
            }

            Vector3 targetPosition = waypoint.position;

            // Keep moving until the wall is very close to the target.
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Ensure exact positioning
            transform.position = targetPosition;
        }

        Debug.Log($"{gameObject.name} has completed its movement sequence.");
    }

    //Method to reset the wall to its original position 
    public void ResetToOriginalPosition()
    {
        if (!isMoving)
        {
            transform.position = originalPosition;
            inPosition = false;
        }
    }

    
}