using TMPro;
using UnityEngine;


public class ScoreController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int score = 0;
    private int HighScore;

    private const string HIGHSCORE = "HighScore";

    private void Start()
    {
        if (highScoreText != null)
            highScoreText.text = HIGHSCORE + PlayerPrefs.GetInt(HIGHSCORE, 0).ToString();
    }

    public void AddScore(int increament)
    {
        score += increament;
        scoreText.text = "" + score;
        GetHighScore();
    }

    public void DecreaseScore(int decreament)
    {
        if (score > 0)
            score -= decreament;
        else score = 0;
        scoreText.text = "" + score;
    }

    public int GetScore() { return score; }

    public int GetHighScore()
    {
        HighScore = PlayerPrefs.GetInt(HIGHSCORE, 0);
        if (score > HighScore)
        {
            PlayerPrefs.SetInt(HIGHSCORE, score);
            PlayerPrefs.Save();
            if (highScoreText != null)
                highScoreText.text = HIGHSCORE + score.ToString();
        }
        return HighScore;
    }
}