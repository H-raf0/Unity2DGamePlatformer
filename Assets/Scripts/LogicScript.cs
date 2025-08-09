using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LogicScript : MonoBehaviour
{
    [SerializeField] private GameObject gameoverObject;
    private GameoverScript GameoverScript;

    public static LogicScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        GameoverScript = gameoverObject.GetComponent<GameoverScript>();
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void GameOver()
    {   
        GameoverScript.GameOver();
    }
    public void RestartGame()
    {
        GameoverScript.RestartGame();
    }
    public void MainManu()
    {
        GameoverScript.MainMenu();
    }
}
