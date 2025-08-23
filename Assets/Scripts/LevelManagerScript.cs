using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManagerScript : MonoBehaviour
{
    public static LevelManagerScript instance;

    [Header("Core References")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Camera mainCamera;

    [Header("UI References")]
    [SerializeField] private GameObject gameoverObject;
    private GameoverScript gameoverScript;

    [Header("Level Configuration")]
    [SerializeField] private int mainMenuBuildIndex = 1;
    [SerializeField] private int firstLevelBuildIndex = 2;

    [Header("Spawning")]
    [Tooltip("The transform where the player should currently spawn.")]
    public Transform currentCheckpoint;

    private int currentLevelIndex;
    private bool isLoading = false;

    #region Unity Lifecycle
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Set target frame rate once.
        Application.targetFrameRate = 60;

        // Get the script from the UI object, if it exists.
        if (gameoverObject != null)
        {
            gameoverScript = gameoverObject.GetComponent<GameoverScript>();
        }

        // Deactivate the player initially, they shouldn't be visible in the menu.
        if (playerObject != null)
        {
            playerObject.SetActive(false);
        }

        // When the game first launches, we are in the ManagerScene.
        // We should immediately load the Main Menu on top of it.
        // We check if the menu is already loaded to prevent issues on a game restart.
        if (!SceneManager.GetSceneByBuildIndex(mainMenuBuildIndex).isLoaded)
        {
            SceneManager.LoadSceneAsync(mainMenuBuildIndex, LoadSceneMode.Additive);
        }
    }
    #endregion



    #region Public Game State Methods

    // This is called by the "Start Game" button in the Main Menu.
    public void StartGame()
    {
        if (isLoading) return;
        StartCoroutine(LoadFirstLevel());
    }

    // This is called by the FinishLineScript.
    public void GoToNextLevel()
    {
        if (isLoading) return;
        StartCoroutine(TransitionToNextLevel());
    }

    public void GameOver()
    {
        if (gameoverScript != null)
        {
            gameoverScript.GameOver();
        }
        else
        {
            Debug.LogError("GameoverScript reference is not set in LevelManagerScript!");
        }
    }

    public void RestartGame()
    {
        if (isLoading) return;

        isLoading = true;

        // Hide the GameOver UI. Your GameoverScript should handle this.
        if (gameoverScript != null)
        {
            gameoverScript.RestartGame();
        }

        // Find the player's script component.
        CharacterScript playerScript = playerObject.GetComponent<CharacterScript>();
        if (playerScript != null)
        {
            // Tell the player character to reset its state (become alive again).
            playerScript.ResetState();
        }

        // Move the player to the last checkpoint.
        // If no checkpoint has been reached, we need a fallback.
        if (currentCheckpoint != null)
        {
            playerObject.transform.position = currentCheckpoint.position;
        }
        else
        {
            // Fallback: If no checkpoint is set, find the level's main StartPoint.
            GameObject startPoint = GameObject.FindWithTag("StartPoint");
            if (startPoint != null)
            {
                playerObject.transform.position = startPoint.transform.localPosition;
            }
            else
            {
                Debug.LogError("Could not restart: No current checkpoint is set and could not find a 'StartPoint' in the level.");
            }
        }

        isLoading = false;
    }

    public void MainMenu()
    {
        if (isLoading) return;
        StartCoroutine(ReturnToMainMenuRoutine());
    }

    private IEnumerator ReturnToMainMenuRoutine()
    {
        isLoading = true;

        
        // Deactivate player so they don't appear in the main menu
        if (playerObject != null)
        {
            playerObject.SetActive(false);
        }

        if (gameoverScript != null)
        {
            gameoverScript.RestartGame();
        }

        // Unload all loaded levels
        for (int i = firstLevelBuildIndex; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (SceneManager.GetSceneByBuildIndex(i).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(i);
            }
        }

        // Load the Main Menu back
        yield return SceneManager.LoadSceneAsync(mainMenuBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(mainMenuBuildIndex));

        isLoading = false;
    }
    #endregion


    #region Scene Loading Coroutines

    private IEnumerator LoadFirstLevel()
    {
        isLoading = true;
        yield return SceneManager.UnloadSceneAsync(mainMenuBuildIndex);

        currentLevelIndex = firstLevelBuildIndex;
        yield return SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentLevelIndex));

        // --- POSITION CAMERA LOGIC --- // 
        MoveCameraToPoint();

        // --- PLAYER SPAWNING LOGIC ---  //
        if (playerObject != null)
        {
            playerObject.SetActive(true);
            GameObject startPoint = GameObject.FindWithTag("StartPoint");
            if (startPoint != null)
            {
                playerObject.transform.position = startPoint.transform.position;
                CharacterScript playerScript = playerObject.GetComponent<CharacterScript>();
                playerScript.ResetState();
            }
            else
            {
                Debug.LogError("Could not find object with tag 'StartPoint' in the first level!");
            }
        }

        if (currentLevelIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            yield return SceneManager.LoadSceneAsync(currentLevelIndex + 1, LoadSceneMode.Additive);
        }

        isLoading = false;
    }

    private IEnumerator TransitionToNextLevel()
    {
        isLoading = true;

        int nextLevelIndex = currentLevelIndex + 1;
        int levelToPreload = nextLevelIndex + 1;
        int levelToUnload = currentLevelIndex;

        if (levelToPreload < SceneManager.sceneCountInBuildSettings)
        {
            yield return SceneManager.LoadSceneAsync(levelToPreload, LoadSceneMode.Additive);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(nextLevelIndex));

        // --- POSITION CAMERA LOGIC --- //
        MoveCameraToPoint();

        // --- PLAYER TELEPORT LOGIC ---
        /*
        GameObject nextStartPoint = GameObject.FindWithTag("StartPoint");
        if (nextStartPoint != null && playerObject != null)
        {
            playerObject.transform.position = nextStartPoint.transform.position;
        }
        else
        {
            Debug.LogError($"Could not find 'StartPoint' in Level {nextLevelIndex}!");
        }*/

        if (SceneManager.GetSceneByBuildIndex(levelToUnload).isLoaded && currentCheckpoint.gameObject.scene == SceneManager.GetActiveScene())
        {
            yield return SceneManager.UnloadSceneAsync(levelToUnload);
        }

        currentLevelIndex++;
        isLoading = false;
    }





    #endregion

    private void MoveCameraToPoint()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Cannot move camera: mainCamera reference is not set in LevelManagerScript.");
            return;
        }

        // Find ALL GameObjects with the "CameraPoint" tag. This will give us an array.
        GameObject[] allCameraPoints = GameObject.FindGameObjectsWithTag("CameraPoint");

        // Get a reference to the scene that is currently active.
        Scene activeScene = SceneManager.GetActiveScene();

        GameObject targetCameraPoint = null;

        // Loop through all the camera points we found.
        foreach (GameObject point in allCameraPoints)
        {
            // Check if the scene this point belongs to is the same as our active scene.
            if (point.scene == activeScene)
            {
                // If it is, we've found our target! Store it and stop searching.
                targetCameraPoint = point;
                break;
            }
        }

        // Now, use the target point we found.
        if (targetCameraPoint != null)
        {
            mainCamera.transform.position = targetCameraPoint.transform.position;
            mainCamera.transform.rotation = targetCameraPoint.transform.rotation;
        }
        else
        {
            // This error will now correctly tell if the ACTIVE scene is missing its camera point.
            Debug.LogError($"Could not find a 'CameraPoint' specifically within the active scene: {activeScene.name}");
        }
    }
}