using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void playGame()
    {
        // Load the game scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Load Game Scene");
    }

    public void options()
    {
        
        Debug.Log("Load Options Scene");
    }
    public void quit()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

}
