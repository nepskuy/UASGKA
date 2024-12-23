using UnityEngine;
using TMPro; // Pastikan Anda menggunakan TextMeshPro untuk UI

public class CarScoreManager : MonoBehaviour
{
    public int currentScore = 0; // Skor yang akan ditampilkan di UI
    public Transform player; // Referensi ke mobil pemain
    public TextMeshProUGUI scoreText; // Referensi ke komponen TextMeshProUGUI untuk menampilkan skor
    public TextMeshProUGUI highestScoreText; // UI untuk menampilkan skor tertinggi

    private float distanceTraveled = 0; // Jarak tempuh pemain
    private bool isGameOver = false; // Status untuk mengetahui apakah permainan sudah berakhir
    private int highestScore = 0; // Skor tertinggi
    private string highScoreKey = "HighScore"; // Key untuk menyimpan skor tertinggi di PlayerPrefs

    void Start()
    {
        // Ambil skor tertinggi yang tersimpan
        highestScore = PlayerPrefs.GetInt(highScoreKey, 0);

        // Tampilkan skor tertinggi di UI
        if (highestScoreText != null)
        {
            highestScoreText.text = "Highest: " + highestScore.ToString();
        }

        // Validasi referensi
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
            return; // Jika permainan sudah berakhir, tidak update skor
        }

        if (player != null && scoreText != null)
        {
            // Update jarak tempuh berdasarkan posisi Z pemain
            distanceTraveled = player.position.z;

            // Update skor berdasarkan jarak tempuh
            currentScore = Mathf.FloorToInt(distanceTraveled);
            scoreText.text = "Score: " + currentScore.ToString(); // Menampilkan skor terbaru

            // Debug log untuk memeriksa skor dan jarak tempuh
            Debug.Log("Distance Traveled: " + distanceTraveled + " Score: " + currentScore);
        }
    }

    public void GameOver()
    {
        isGameOver = true; // Set status game over
        CheckAndUpdateHighScore();
        Debug.Log("Game Over! Final Score: " + currentScore);
    }

    private void CheckAndUpdateHighScore()
    {
        // Cek apakah skor saat ini lebih tinggi dari skor tertinggi
        if (currentScore > highestScore)
        {
            highestScore = currentScore;
            PlayerPrefs.SetInt(highScoreKey, highestScore); // Simpan skor tertinggi

            // Update UI skor tertinggi
            if (highestScoreText != null)
            {
                highestScoreText.text = "Highest: " + highestScore.ToString();
            }

            Debug.Log("New High Score: " + highestScore);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        distanceTraveled = 0;
        if (scoreText != null)
        {
            scoreText.text = "Score: 0"; // Reset teks di UI
        }
        Debug.Log("Score reset to 0.");
    }

    public int GetScore()
    {
        return currentScore;
    }

    public void ResetHighScore()
    {
        highestScore = 0;
        PlayerPrefs.SetInt(highScoreKey, 0); // Reset skor tertinggi di PlayerPrefs
        if (highestScoreText != null)
        {
            highestScoreText.text = "Highest: 0"; // Reset teks di UI
        }
        Debug.Log("High Score reset to 0.");
    }
}
