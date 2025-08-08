using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLineScript : MonoBehaviour
{
    /*private LogicScript logic = LogicScript.instance;
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }*/

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Check if the player touched the finish line
        {
            Debug.Log("🎉 You won! 🎉");  // Print to console
            LogicScript.instance.LoadNextLevel();  // Load the next level
        }
    }
}
