using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    private AudioSource musicSource;
    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
    }
    public void ToggleMute()
    {
        musicSource.mute = !musicSource.mute;
    }
}
