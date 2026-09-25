using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public AudioSource footstepAudio;
    public Transform flashlight;
    public bool gameEnded = false;

    private Rigidbody2D playerRigidbody;
    private Vector2 movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Stop movement after the player reaches the exit.
        if (gameEnded)
        {
            return;
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Rotate the flashlight toward direction the player faces.
        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            flashlight.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        if(movement.magnitude > 0 && !footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
        else if (movement.magnitude == 0 && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }

    void FixedUpdate()
    {
        // Move the player using physics in FixedUpdate.
        playerRigidbody.MovePosition(playerRigidbody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
