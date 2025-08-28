using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;  // Higher = faster (duration ≈ 1 / moveSpeed)

    [Header("Grouping")]
    public int groupID = 0; // Spikes with the same groupID trigger together
    [Header("Enable/disable the trigger")]
    public bool canTrigger = true;

    private bool hasMoved = false;                  // Prevent multiple triggers
    private float multiplier = 1f;                  // Travel distance in "direction" units
    private Vector2 direction;
    // Activates all spikes that share this spike's groupID.
    private void ActivateGroup()
    {
        // Updated to use FindObjectsByType with FindObjectsSortMode.None for better performance.
        SpikeScript[] all = FindObjectsByType<SpikeScript>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var spike in all)
        {
            if (spike.groupID == this.groupID)
            {
                spike.ActivateFromGroup();
            }
        }
    }

    // Called when the group triggers this spike (or if this spike is the origin).
    // Ensures per-spike setup runs, but only once.
    private void ActivateFromGroup()
    {
        if (hasMoved) return;

        hasMoved = true;

        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();

        if (circleCollider != null)
        {
            float offsetX = circleCollider.offset.x;
            if (offsetX < -1f)
            {
                Debug.Log("Circle Collider Offset X: " + offsetX + " on " + name);
                multiplier = 8f;  // move farther
                moveSpeed = 2f;   // move slower
            }
        }

        // Start the movement for this spike.
        StartCoroutine(MoveSpike());
    }
    private IEnumerator MoveSpike()
    {
        Vector2 startPosition = transform.localPosition;

        // Robust direction: snap Z to nearest 90 degrees (0, 90, 180, 270).
        float z = Mathf.Round(transform.localEulerAngles.z);

        switch (z)
        {
            case 0: direction = Vector2.left; break;
            case 90: direction = Vector2.down; break;
            case 180: direction = Vector2.right; break;
            case 270: direction = Vector2.up; break;
            default:
                Debug.Log("error in rotation value");
                break;
        }

        Debug.Log($"[{name}] Z:{z:F2}, dir:{direction}");

        Vector2 targetPosition = startPosition + direction * multiplier;

        float t = 0f;
        while (t < 1f)
        {
            // Lerp parameter t grows at moveSpeed per second ⇒ duration ≈ 1 / moveSpeed.
            transform.localPosition = Vector2.Lerp(startPosition, targetPosition, t);
            t += Time.deltaTime * moveSpeed;
            yield return null;
        }

        transform.localPosition = targetPosition; // ensure exact final position
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canTrigger)
        {
            if (other.CompareTag("Player") && !hasMoved)
            {
                // When one is hit, activate the whole group.
                if (this.groupID != 0)
                {
                    ActivateGroup();
                }
                else
                {
                    ActivateFromGroup(); // If groupID is 0, this spike is not in a group.
                }
            }
        }
    }
}
