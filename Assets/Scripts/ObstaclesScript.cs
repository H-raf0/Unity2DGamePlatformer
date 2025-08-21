using UnityEngine;

public class ObstaclesScript : MonoBehaviour
{

    void Start()
    {
        SpikeScript[] spikes = GameObject.FindObjectsByType<SpikeScript>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (SpikeScript spike in spikes)
        {
            spike.transform.Translate(Vector2.right);
        }
    }
}
