using UnityEngine;

public class PlayerWin : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject monster;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            // Check if the player has reached the exit.
            PlayerMovement movement = GetComponent<PlayerMovement>();

            // Disable player movement after escaping.
            if (movement != null)
            {
                movement.enabled = false;
            }

            // Display the "YOU ESCAPED!" screen.
            winPanel.SetActive(true);

            // Stop the monster after the player escapes.
            if (monster != null)
            {
                monster.SetActive(false);
            }
        }
    }
    
}
