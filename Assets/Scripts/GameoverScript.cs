using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameoverScript : MonoBehaviour
{
    public void GameOver()
    {
        gameObject.SetActive(true);  // Activate the game over UI
    }
    public void RestartGame()
    {
        gameObject.SetActive(false);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        gameObject.SetActive(false);
    }
}
