using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the game scene
        LevelManagerScript.instance.StartGame();
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
