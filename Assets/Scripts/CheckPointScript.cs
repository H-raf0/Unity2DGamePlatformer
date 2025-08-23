using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Call the dedicated function on the manager to set this as the new checkpoint.
            LevelManagerScript.instance.SetNewCheckpoint(this.transform);

            // To prevent it from triggering repeatedly, you can disable its collider.
            GetComponent<Collider2D>().enabled = false;
        }
    }
}