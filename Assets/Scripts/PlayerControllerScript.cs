using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    [Header("Mobile Controls State")]
    public bool moveLeft = false;
    public bool moveRight = false;
    public bool jump = false;

    private void Start()
    {
        // Explicitly initialize to ensure clean state
        moveLeft = false;
        moveRight = false;
        jump = false;
    }

    // Button event methods for UI buttons
    public void OnLeftButtonDown() => moveLeft = true;
    public void OnLeftButtonUp() => moveLeft = false;

    public void OnRightButtonDown() => moveRight = true;
    public void OnRightButtonUp() => moveRight = false;

    public void OnJumpButtonDown() => jump = true;
    public void OnJumpButtonUp() => jump = false;

    // Additional methods for debugging on Android
    public void ForceStopAllInput()
    {
        moveLeft = false;
        moveRight = false;
        jump = false;
    }

    public bool IsAnyInputActive()
    {
        return moveLeft || moveRight || jump;
    }
}