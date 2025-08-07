using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LogicScript : MonoBehaviour
{
    public GameObject gameoverScreen;
    public Button restartButton;
    public GameoverScript GameoverScript;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
  
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void GameOver()
    {
        GameoverScript.GameOver();
    }
}
