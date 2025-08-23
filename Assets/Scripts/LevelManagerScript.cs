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
        StartCoroutine(RestartLevelRoutine());
    }

    public void MainMenu()
    {
        if (isLoading) return;
        StartCoroutine(ReturnToMainMenuRoutine());
    }

    public void SetNewCheckpoint(Transform newCheckpoint)
    {
        Debug.Log($"New checkpoint set at {newCheckpoint.name} in scene {newCheckpoint.gameObject.scene.name}");
        if (currentCheckpoint != null) UnloadScene(currentCheckpoint.gameObject.scene); // Unload the scene of the old checkpoint to avoid memory leaks.
        currentCheckpoint = newCheckpoint;
    }

    public void UnloadScene(Scene scene)
    {
        Debug.Log($"Unloading scene: {scene.name} with the old check point (Build Index: {scene.buildIndex})");
        StartCoroutine(UnloadSceneRoutine(scene));
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

        // Preload the next part of the world (e.g., Level 3).
        if (levelToPreload < SceneManager.sceneCountInBuildSettings)
        {
            yield return SceneManager.LoadSceneAsync(levelToPreload, LoadSceneMode.Additive);
        }

        // The player has crossed the boundary. Set the new level as "active".
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(nextLevelIndex));

        // Move the camera to the CameraPoint of the new active scene.
        MoveCameraToPoint();

        // Unload the part of the world that is now behind the player.
        Scene sceneToUnload = SceneManager.GetSceneByBuildIndex(levelToUnload);
        if (sceneToUnload.isLoaded && sceneToUnload != currentCheckpoint.gameObject.scene)
        {
            yield return SceneManager.UnloadSceneAsync(levelToUnload);
        }

        // Update our master index.
        currentLevelIndex++;
        isLoading = false;
    }

    private IEnumerator RestartLevelRoutine()
    {
        isLoading = true;
        Debug.Log("--- SMART RESTART INITIATED ---");

        // Hide the GameOver UI.
        if (gameoverScript != null)
        {
            gameoverScript.RestartGame();
        }

        // Determine which scene to restart
        int sceneToRestartIndex;
        Vector3 respawnPosition;

        // Check if a checkpoint has been triggered and is still valid (its scene is loaded).
        if (currentCheckpoint != null && currentCheckpoint.gameObject.scene.isLoaded)
        {
            sceneToRestartIndex = currentCheckpoint.gameObject.scene.buildIndex;
            respawnPosition = currentCheckpoint.position;
            Debug.Log($"Restarting at checkpoint in scene {sceneToRestartIndex}");
        }
        else
        {
            // FALLBACK: If no checkpoint, restart the level the player is currently in.
            sceneToRestartIndex = currentLevelIndex;
            // We'll find the start point after the scene reloads.
            respawnPosition = Vector3.zero; // Temporary value
            Debug.Log($"No valid checkpoint. Restarting current scene {sceneToRestartIndex}");
        }
        // -----------------------------------------

        // Unload ALL currently loaded game levels for a clean slate.
        for (int i = firstLevelBuildIndex; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (SceneManager.GetSceneByBuildIndex(i).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(i);
            }
        }

        // Load the correct level that contains our checkpoint.
        yield return SceneManager.LoadSceneAsync(sceneToRestartIndex, LoadSceneMode.Additive);

        // Pre-load the level that comes after it for seamless transition.
        if (sceneToRestartIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            yield return SceneManager.LoadSceneAsync(sceneToRestartIndex + 1, LoadSceneMode.Additive);
        }

        // Update the game state to reflect the new reality.
        currentLevelIndex = sceneToRestartIndex;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentLevelIndex));

        // Move camera and player.
        MoveCameraToPoint();

        // If we are using the fallback, we need to find the start point now.
        if (respawnPosition == Vector3.zero)
        {
            GameObject startPoint = FindObjectInActiveScene("StartPoint");
            if (startPoint != null)
            {
                respawnPosition = startPoint.transform.position;
                // Also update the checkpoint to this start point.
                currentCheckpoint = startPoint.transform;
            }
        }

        playerObject.transform.position = respawnPosition;

        // Reset the player's internal state.
        CharacterScript playerScript = playerObject.GetComponent<CharacterScript>();
        if (playerScript != null)
        {
            playerScript.ResetState();
        }

        isLoading = false;
        Debug.Log("--- SMART RESTART COMPLETE ---");
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

    private IEnumerator UnloadSceneRoutine(Scene scene)
    {
        yield return SceneManager.UnloadSceneAsync(scene);
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

    // Helper function to find a tagged object only within the current active scene.
    private GameObject FindObjectInActiveScene(string tag)
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tag);
        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene)
            {
                return obj;
            }
        }
        return null; // Return null if nothing is found in the active scene
    }
}