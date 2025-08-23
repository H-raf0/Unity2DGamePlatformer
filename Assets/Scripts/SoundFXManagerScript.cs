using UnityEngine;

public class SoundFXManagerScript : MonoBehaviour
{
    public static SoundFXManagerScript instance;

    [SerializeField] private AudioSource soundFXObjectPrefab;

    private void Awake()
    {
        // Singleton Pattern
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

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Spawn a new AudioSource from the prefab at the desired position
        AudioSource audioSource = Instantiate(soundFXObjectPrefab, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        // Destroy the temporary sound object after the clip has finished playing
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}