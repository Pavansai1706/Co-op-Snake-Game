using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private Vector2 _screenBounds;
    private int _score = 0;
    private int _highScore = 0; // Add high score variable
    private bool _isGameOver = false;

    private List<Transform> _segments = new List<Transform>();
    [SerializeField] private int initialSize = 3;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText; // Reference to high score text
    [SerializeField] private GameObject gameOverPanel; // Reference to game over UI panel

    private void Start()
    {
        ResetState();

        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        // Load high score from PlayerPrefs
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

            // Calculate the new position based on the current position and direction
            Vector3 newPosition = transform.position + (Vector3)_direction;

            // Wrap around the screen if the snake moves beyond the screen boundaries
            if (newPosition.x > _screenBounds.x)
                newPosition.x = -_screenBounds.x;
            else if (newPosition.x < -_screenBounds.x)
                newPosition.x = _screenBounds.x;

            if (newPosition.y > _screenBounds.y)
                newPosition.y = -_screenBounds.y;
            else if (newPosition.y < -_screenBounds.y)
                newPosition.y = _screenBounds.y;

            // Update the position
            transform.position = newPosition;
        }
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);

        // Increment score
        _score += scoreValue;
        UpdateScoreText();

        // Update high score if needed
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
        _segments.Add(this.transform);

        for (int i = 1; i < this.initialSize; i++)
        {
            _segments.Add(Instantiate(this.segmentPrefab));
        }

        this.transform.position = Vector3.zero;

        // Reset score
        _score = 0;
        UpdateScoreText();

        // Reset game over state
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
            if (other.tag == "Food")
            {
                Grow();
            }
            else if (other.tag == "Obstacle" || other.tag == "SnakeBody")
            {
                GameOver();
            }
        }
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
        SaveHighScore(); // Save high score when game over
    }

    private void OnDestroy()
    {
        SaveHighScore(); // Save high score when the game object is destroyed (e.g., when quitting the game)
    }
}
