using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    public bool moveLeft;
    public bool moveRight;
    public bool jump;

    public void OnLeftButtonDown() => moveLeft = true;
    public void OnLeftButtonUp() => moveLeft = false;

    public void OnRightButtonDown() => moveRight = true;
    public void OnRightButtonUp() => moveRight = false;

    public void OnJumpButtonDown() => jump = true;
    public void OnJumpButtonUp() => jump = false;
}
