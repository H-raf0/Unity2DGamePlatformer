using UnityEngine;
//using UnityEngine.SceneManagement;

public class FinishLineScript : MonoBehaviour
{
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Check if the player touched the finish line
        {   

            Debug.Log("You won!");  // Print to console
            LogicScript.instance.LoadNextLevel();  // Load the next level
        }
    }
}
