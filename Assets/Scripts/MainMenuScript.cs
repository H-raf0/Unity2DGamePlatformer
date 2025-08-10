using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the game scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Load Game Scene");
    }

    public void Options()
    {
        
        Debug.Log("Load Options Scene");
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

}
