using UnityEngine;

public class swingAxeScript : MonoBehaviour
{
    [Header("Knockback Settings")]
    [Tooltip("The horizontal strength of the push.")]
    [SerializeField] private float horizontalForce = 40f;
    [Tooltip("The vertical strength of the push.")]
    [SerializeField] private float verticalForce = 20f;
    [Tooltip("How long the player loses control after being hit.")]
    [SerializeField] private float knockbackDuration = 2f;

    private int axeDirection = -1; // 1 for right, -1 for left

    private void Update()
    {
        if(Mathf.Approximately(transform.eulerAngles.z, 90) || Mathf.Approximately(transform.eulerAngles.z, 270))
        {
            axeDirection *= -1;
            //Debug.Log("Direction changed to: " + axeDirection);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterScript player = collision.GetComponent<CharacterScript>();
            if (player != null)
            {
                // --- Determine Direction ---
                //float axeDirection = (player.transform.position.x < transform.position.x) ? -1f : 1f;

                // --- Create the Force Vector ---
                Vector2 knockbackForce = new Vector2(horizontalForce * axeDirection, verticalForce);

                // --- Tell the Player to Get Knocked Back ---
                player.ApplyKnockback(knockbackForce, knockbackDuration);
            }
        }
    }
}