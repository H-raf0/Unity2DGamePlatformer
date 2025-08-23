using UnityEngine;

public class FinishLineScript : MonoBehaviour
{
    private bool hasFinished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasFinished)
        {
            hasFinished = true;
            Debug.Log("Level Complete! Transitioning...");

            // Optionally disable player movement here if you want
            // other.GetComponent<CharacterScript>().enabled = false;

            // Tell the LevelManager to handle the transition
            LevelManagerScript.instance.GoToNextLevel();
        }
    }
}