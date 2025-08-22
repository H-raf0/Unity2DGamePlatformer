using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LogicScript : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameoverObject;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform uiTransform;
    
    [Header("Spawning")]
    [Tooltip("The transform where the player should currently spawn.")]
    public Transform currentCheckpoint;

    public static LogicScript instance;

    private GameoverScript GameoverScript;


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
        Debug.Log("loading next scene\n");
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
