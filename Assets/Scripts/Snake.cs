using UnityEngine;
using TMPro;
using System.Collections;
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
    private bool _isScoreBoostActive = false;
    private float _speedUpDuration = 5f;
    private float _speedUpMultiplier = 2f;
    private float _scoreBoostDuration = 10f;
    private bool _isCooldownActive = false;
    private float _deathCooldownDuration = 2f;

    [SerializeField] private int initialSize = 3;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private int foodScoreValue = 10; // Score value for food
    [SerializeField] private int scoreBoosterMultiplier = 2; // Multiplier for score booster
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
            float movementSpeed = (_isSpeedUpActive) ? _speedUpMultiplier : 1f;

            for (int i = _segments.Count - 1; i > 0; i--)
            {
                _segments[i].position = _segments[i - 1].position;
            }

            Vector3 newPosition = transform.position + (Vector3)_direction * movementSpeed;

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

        // Update the segments list to include the new segment
        _segments[_segments.Count - 1].position = _segments[_segments.Count - 2].position;

        IncrementScore(_isScoreBoostActive ? foodScoreValue * scoreBoosterMultiplier : foodScoreValue); // Increment score with score booster effect
    }


    private void IncrementScore(int value)
    {
        _score += value;
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
            if (!_isShieldActive)
            {
                if (other.CompareTag("Food"))
                {
                    Grow();
                }
                else if (other.CompareTag("Obstacle") || other.CompareTag("Player"))
                {
                    if (!_isCooldownActive)
                    {
                        GameOver();
                    }
                }
            }
            else if (other.CompareTag("PowerUp"))
            {
                PowerUp powerUp = other.GetComponent<PowerUp>();
                if (powerUp != null)
                {
                    // Check if the power-up is a score booster
                    if (powerUp.powerUpType == PowerUp.PowerUpType.ScoreBoost)
                    {
                        ActivateScoreBoost(); // Activate the score booster
                    }
                    
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

    private IEnumerator DeactivateShieldCoroutine()
    {
        yield return new WaitForSeconds(_deathCooldownDuration);
        _isShieldActive = false;
        Debug.Log("Shield deactivated!");
    }

    private IEnumerator DeactivateSpeedUpCoroutine()
    {
        yield return new WaitForSeconds(_speedUpDuration);
        _isSpeedUpActive = false;
        Debug.Log("Speed Up deactivated!");
    }

    private IEnumerator DeactivateScoreBoostCoroutine()
    {
        yield return new WaitForSeconds(_scoreBoostDuration);
        _isScoreBoostActive = false;
        Debug.Log("Score Boost deactivated!");
    }

    private void ActivateShield()
    {
        _isShieldActive = true;
        Debug.Log("Shield activated!");
        StartCoroutine(DeactivateShieldCoroutine());
    }

    private void ActivateSpeedUp()
    {
        _isSpeedUpActive = true;
        Debug.Log("Speed Up activated!");
        StartCoroutine(DeactivateSpeedUpCoroutine());
    }

    private void ActivateScoreBoost()
    {
        if (!_isScoreBoostActive) // Check if score boost is not already active
        {
            _isScoreBoostActive = true;
            Debug.Log("Score Boost activated!");
            IncrementScore(foodScoreValue); // Increment score with +10
            Grow(); // Grow the snake
            StartCoroutine(DeactivateScoreBoostCoroutine());
        }
        else
        {
            // Score boost is already active
            Debug.Log("Score Boost is already active!");
        }
    }

    private void DeactivateScoreBoost()
    {
        _isScoreBoostActive = false;
        Debug.Log("Score Boost deactivated!");
    }

    private IEnumerator DeathCooldown()
    {
        _isCooldownActive = true;
        yield return new WaitForSeconds(_deathCooldownDuration);
        _isCooldownActive = false;
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
