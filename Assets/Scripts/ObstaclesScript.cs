using UnityEngine;

public class ObstaclesScript : MonoBehaviour
{

    void Start()
    {
        // Only look at SpikeScripts that are children of THIS Obstacles object
        SpikeScript[] spikes = GetComponentsInChildren<SpikeScript>();

        foreach (SpikeScript spike in spikes)
        {
            spike.transform.Translate(Vector2.right);
        }
    }
}
