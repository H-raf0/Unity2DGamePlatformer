using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // When the player touches this checkpoint, tell the LevelManager 
            // to store THIS transform as the new restart location.
            LevelManagerScript.instance.currentCheckpoint = this.transform;

            Debug.Log("Checkpoint reached!");

            // ---------------TO DO :   unload the scene with the last checkpoint if it exist------------------------

            // Optionally, disable the collider after use so the log isn't spammed.
            // GetComponent<Collider2D>().enabled = false;
        }
    }
}