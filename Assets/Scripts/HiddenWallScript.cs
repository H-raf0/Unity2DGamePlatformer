using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenWallScript : MonoBehaviour
{
    private TilemapRenderer wallTileMapRenderer;

    // This ensures the trigger only works once until the wall has reappeared.
    private bool isInvisible = false;


    private void Awake()
    {
        // If the user forgot to assign the SpriteRenderer, try to find it on this GameObject.
        if (wallTileMapRenderer == null)
        {
            wallTileMapRenderer = GetComponent<TilemapRenderer>();
        }

        if (wallTileMapRenderer == null)
        {
            Debug.LogError("HiddenWallScript requires a TilemapRenderer component on the same GameObject, but none was found.", gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if it's the player and if the wall isn't already invisible.
        if (collision.CompareTag("Player") && !isInvisible)
        {
            Debug.Log("Player triggered an illusionary wall!");

            // Make the wall invisible.
            MakeInvisible();
        }
    }

    private void MakeInvisible()
    {
        isInvisible = true;
        if (wallTileMapRenderer != null)
        {
            wallTileMapRenderer.enabled = false;
        }
    }
}
