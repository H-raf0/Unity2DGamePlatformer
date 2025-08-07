using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameoverScript : MonoBehaviour
{
    public Button restartButton;
    public Button mainMenuButton;
    public void GameOver()
    {
        gameObject.SetActive(true);  // Activate the game over UI

        // Clear any previous selections
        EventSystem.current.SetSelectedGameObject(null);

        // Set a new default selection, pressing "enter" will trigger the selection
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
    }
}
