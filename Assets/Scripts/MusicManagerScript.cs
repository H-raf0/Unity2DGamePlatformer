using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    public static MusicManagerScript instance;
    private AudioSource musicSource;

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

        musicSource = GetComponent<AudioSource>();
    }

    public void ToggleMute()
    {
        musicSource.mute = !musicSource.mute;
    }
}