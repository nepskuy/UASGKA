using UnityEngine;
using TMPro;

public class CarScoreManager : MonoBehaviour
{
    public int currentScore = 0;
    public Transform player;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highestScoreText;

    private float distanceTraveled = 0;
    private bool isGameOver = false;
    private int highestScore = 0;
    private string highScoreKey = "HighScore";

    void Start()
    {
        highestScore = PlayerPrefs.GetInt(highScoreKey, 0);

        if (highestScoreText != null)
        {
            highestScoreText.text = "Highest: " + highestScore.ToString();
        }

        if (scoreText == null)
        {
            Debug.LogError("scoreText is not assigned!");
        }

        if (highestScoreText == null)
        {
            Debug.LogError("highestScoreText is not assigned!");
        }

        if (player == null)
        {
            Debug.LogError("Player is not assigned in CarScoreManager!");
        }
    }

    void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (player != null && scoreText != null)
        {
            distanceTraveled = player.position.z;
            currentScore = Mathf.FloorToInt(distanceTraveled);
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        CheckAndUpdateHighScore();
    }

    private void CheckAndUpdateHighScore()
    {
        if (currentScore > highestScore)
        {
            highestScore = currentScore;
            PlayerPrefs.SetInt(highScoreKey, highestScore);

            if (highestScoreText != null)
            {
                highestScoreText.text = "Highest: " + highestScore.ToString();
            }
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        distanceTraveled = 0;
        if (scoreText != null)
        {
            scoreText.text = "Score: 0";
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    public void ResetHighScore()
    {
        highestScore = 0;
        PlayerPrefs.SetInt(highScoreKey, 0);
        PlayerPrefs.Save();
        if (highestScoreText != null)
        {
            highestScoreText.text = "Highest: 0";
        }

        FindObjectOfType<UIManager>().ResumeGame();
    }

    public void ResetHighScoreButton()
    {
        ResetHighScore();
    }
}
