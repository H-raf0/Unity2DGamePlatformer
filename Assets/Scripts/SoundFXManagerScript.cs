using UnityEngine;

public class SoundFXManagerScript : MonoBehaviour
{
    public static SoundFXManagerScript instance;

    private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;    
        }
        soundFXObject = GetComponent<AudioSource>();
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // sapwn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // assign audioClip
        audioSource.clip = audioClip;

        // assign volume
        audioSource.volume = volume;

        // play sound
        audioSource.Play();

        // get length of sound FX clip
        float clipLength = audioSource.clip.length;

        // destroy gameObject after clip length
        Destroy(audioSource.gameObject, clipLength);
    }
}
