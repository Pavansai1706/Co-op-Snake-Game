using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private Vector2 _screenBounds;
    private int _score = 0;
    private int _highScore = 0;
    private bool _isGameOver = false;
    private bool _isShieldActive = false;
    private bool _isSpeedUpActive = false;
    private bool _isScoreBoostActive = false; // Added score boost variable
    private float _speedUpDuration = 5f;
    private float _speedUpMultiplier = 2f;
    private float _scoreBoostDuration = 10f; // Duration of score boost power-up in seconds

    [SerializeField] private int initialSize = 3;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject gameOverPanel;

    private List<Transform> _segments = new List<Transform>();

    private void Start()
    {
        ResetState();
        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();
    }

    private void Update()
    {
        if (!_isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _direction = Vector2.up;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                _direction = Vector2.down;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                _direction = Vector2.left;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                _direction = Vector2.right;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isGameOver)
        {
            for (int i = _segments.Count - 1; i > 0; i--)
            {
                _segments[i].position = _segments[i - 1].position;
            }

            Vector3 newPosition = transform.position + (Vector3)_direction;

            if (newPosition.x > _screenBounds.x)
                newPosition.x = -_screenBounds.x;
            else if (newPosition.x < -_screenBounds.x)
                newPosition.x = _screenBounds.x;

            if (newPosition.y > _screenBounds.y)
                newPosition.y = -_screenBounds.y;
            else if (newPosition.y < -_screenBounds.y)
                newPosition.y = _screenBounds.y;

            transform.position = newPosition;
        }
    }

    private void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment);

        // Increment score with or without score boost
        _score += _isScoreBoostActive ? scoreValue * 2 : scoreValue;
        UpdateScoreText();

        if (_score > _highScore)
        {
            _highScore = _score;
            UpdateHighScoreText();
        }
    }

    private void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(transform);

        for (int i = 1; i < initialSize; i++)
        {
            _segments.Add(Instantiate(segmentPrefab));
        }

        transform.position = Vector3.zero;

        _score = 0;
        UpdateScoreText();
        _isGameOver = false;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isGameOver)
        {
            if (other.CompareTag("Food"))
            {
                Grow();
            }
            else if (other.CompareTag("Obstacle") || other.CompareTag("Player"))
            {
                GameOver();
            }
            else if (other.CompareTag("PowerUp"))
            {
                PowerUp powerUp = other.GetComponent<PowerUp>();
                if (powerUp != null)
                {
                    ActivatePowerUp(powerUp.powerUpType);
                    Destroy(other.gameObject);
                }
            }
        }
    }

    public void ActivatePowerUp(PowerUp.PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUp.PowerUpType.Shield:
                ActivateShield();
                break;
            case PowerUp.PowerUpType.ScoreBoost:
                ActivateScoreBoost();
                break;
            case PowerUp.PowerUpType.Speed:
                ActivateSpeedUp();
                break;
        }
    }

    private void ActivateShield()
    {
        _isShieldActive = true;
        Debug.Log("Shield activated!");
        // Add code here to visually represent the shield effect if needed
    }

    private void ActivateSpeedUp()
    {
        _isSpeedUpActive = true;
        _speedUpMultiplier = 2.0f; // Double the speed
        Invoke("DeactivateSpeedUp", _speedUpDuration); // Deactivate speed-up after duration
    }

    private void DeactivateSpeedUp()
    {
        _isSpeedUpActive = false;
        _speedUpMultiplier = 1.0f; // Reset speed to normal
    }

    private void ActivateScoreBoost()
    {
        _isScoreBoostActive = true;
        Invoke("DeactivateScoreBoost", _scoreBoostDuration); // Deactivate score boost after duration
    }

    private void DeactivateScoreBoost()
    {
        _isScoreBoostActive = false;
    }


    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + _score.ToString();
        }
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + _highScore.ToString();
        }
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", _highScore);
        PlayerPrefs.Save();
    }

    private void GameOver()
    {
        _isGameOver = true;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        SaveHighScore();
    }

    private void OnDestroy()
    {
        SaveHighScore();
    }
}
