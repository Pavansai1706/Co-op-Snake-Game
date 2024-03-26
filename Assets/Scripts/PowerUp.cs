using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Shield,
        ScoreBoost,
        Speed
    }

    public PowerUpType powerUpType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Snake snake = collision.GetComponent<Snake>();
            if (snake != null)
            {
                // Instead of calling methods directly, let the Snake class handle power-up activation
                snake.ActivatePowerUp(powerUpType);

                // Deactivate the power-up object instead of destroying it
                gameObject.SetActive(false);
            }
        }
    }
}

