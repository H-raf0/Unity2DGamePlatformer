using UnityEngine;

public class UserUiScript : MonoBehaviour
{
    public static UserUiScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicates
        }
    }
}
