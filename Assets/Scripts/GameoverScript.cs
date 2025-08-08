using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameoverScript : MonoBehaviour
{
    public void GameOver()
    {
        gameObject.SetActive(true);  // Activate the game over UI
    }
}
