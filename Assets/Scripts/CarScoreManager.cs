using UnityEngine;
using TMPro; // Pastikan Anda menggunakan TextMeshPro untuk UI

public class CarScoreManager : MonoBehaviour
{
    public int currentScore = 0; // Skor yang akan ditampilkan di UI
    public Transform player; // Referensi ke mobil pemain
    public TextMeshProUGUI scoreText; // Referensi ke komponen TextMeshProUGUI untuk menampilkan skor

    private float distanceTraveled = 0; // Jarak tempuh pemain

    void Start()
    {
        // Pastikan player dan scoreText sudah terhubung dengan benar di inspector
        if (scoreText == null)
        {
            Debug.LogError("scoreText is not assigned!");
        }

        if (player == null)
        {
            Debug.LogError("Player is not assigned in CarScoreManager!");
        }
    }

    void Update()
    {
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

    public void AddScore(int value)
    {
        currentScore += value;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
        Debug.Log("Added Score: " + value + " Total Score: " + currentScore);
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
}
