using NUnit.Framework.Constraints;
using UnityEngine;

public class PushWallConditionScript1 : MonoBehaviour
{
    [SerializeField] private Transform axeRotation;
    [SerializeField] private GameObject pushWall;

    [Header("in Zone Detection")]
    [Tooltip("How long the player must be in the zone.")]
    [SerializeField] private float inZoneDuration = 2.0f; // Player must be still for 2 seconds

    private Vector3 lastPlayerPosition;
    private float inZoneTimer = 0f;
    private bool playerInZone = false;

    // Keep track of the player GameObject
    private GameObject player;


    private Quaternion targetRotation = Quaternion.Euler(0f, 0f, -90f);

    // Update is called once per frame
    void Update()
    {
        // --- Find the Player ---
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Initialize the last known position when the player is first found.
                lastPlayerPosition = player.transform.position;
            }
        }

        if (player == null) return;

        float distanceMoved = Vector3.Distance(player.transform.position, lastPlayerPosition);


        if (distanceMoved < 1f) 
        {
            inZoneTimer += Time.deltaTime;

            if (inZoneTimer >= inZoneDuration)
            {
                playerInZone = true;
            }
        }
        else
        {
            inZoneTimer = 0f;
            playerInZone = false;
        }

        // Update the last known position for the next frame's check.
        lastPlayerPosition = player.transform.position;

        // --- Axe Rotation Check ---
        // The wall only moves if the axe is in position AND the player is idle.
        if (Quaternion.Angle(axeRotation.rotation, targetRotation) < 0.1f && playerInZone)
        {
            pushWall.GetComponent<PushWallScript>().conditionMet = true;
        }
        else
        {
            pushWall.GetComponent<PushWallScript>().conditionMet = false;
        }
    }
}
