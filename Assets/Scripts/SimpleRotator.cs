using UnityEngine;
using System.Collections;

public class SimpleRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The target angle in degrees on the Z-axis the object will rotate to.")]
    [SerializeField] private float targetAngle = 90f;

    [Tooltip("How fast the object rotates in degrees per second.")]
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Testing")]
    [Tooltip("If checked, the object will automatically rotate when the game starts.")]
    [SerializeField] private bool rotateOnStart = false;

    // Keep track of the running coroutine to prevent overlaps
    private Coroutine rotationCoroutine;

    private void Start()
    {
        // If the testing checkbox is ticked, start the rotation immediately.
        if (rotateOnStart)
        {
            StartRotation();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger has the "Player" tag.
        if (collision.CompareTag("Player"))
        {
            StartRotation();
        }
    }

    // This is a public function that other scripts or UI buttons can call.
    public void StartRotation()
    {
        // If a rotation is already happening, stop it first to start the new one cleanly.
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        // Start the new rotation and store a reference to it.
        rotationCoroutine = StartCoroutine(RotateToTarget());
    }

    // This coroutine handles the actual rotation over several frames.
    private IEnumerator RotateToTarget()
    {
        // 1. Calculate the target rotation as a Quaternion.
        // Quaternions are what Unity uses internally for all rotations.
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        // 2. Loop every frame until the object's current rotation is very close to the target.
        // We check the angle between them. It's safer than checking for direct equality.
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            // 3. Move the object's current rotation a little bit closer to the target.
            // Quaternion.RotateTowards is the perfect tool for this. It's like Vector3.MoveTowards but for rotation.
            // It moves at a maximum speed of 'rotateSpeed * Time.deltaTime' degrees per frame.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            // 4. Pause the function here and wait until the next frame to continue the loop.
            yield return null;
        }

        // 5. Once the loop is finished, snap to the exact target rotation.
        // This corrects any tiny errors left over from the loop.
        transform.rotation = targetRotation;

        Debug.Log("Rotation complete!");
        rotationCoroutine = null; // Clear the coroutine reference
    }
}