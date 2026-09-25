using UnityEngine;

public class PlayerCaught : MonoBehaviour
{
    public GameObject caughtPanel;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            caughtPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
