using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LogicScript : MonoBehaviour
{
    public GameObject gameoverScreen;
    public Button restartButton;

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
        gameoverScreen.SetActive(true);

        // Clear any previous selections
        EventSystem.current.SetSelectedGameObject(null);

        // Set a new default selection, pressing "enter" will trigger the selection
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
    }
}
