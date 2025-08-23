using UnityEngine;

public class ObstaclesScript : MonoBehaviour
{
    void Start()
    {
        // only children of THIS GameObject (and their children)
        SpikeScript[] spikes = GetComponentsInChildren<SpikeScript>();

        foreach (SpikeScript spike in spikes)
        {
            spike.transform.Translate(Vector2.right);
        }
    }
}

