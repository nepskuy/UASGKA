using UnityEngine;
using TMPro; // Pastikan ini ada di bagian atas

public class CarScoreManager : MonoBehaviour
{
    private int score;
    public Transform player; // Referensi ke mobil
    public TextMeshProUGUI scoreText; // Teks untuk skor

    private float distanceTraveled = 0;

    // Pastikan hanya ada satu metode Update
    void Update()
    {
        if (player != null)
        {
            distanceTraveled = player.position.z; // Hitung skor dari posisi Z
            scoreText.text = "Score: " + Mathf.FloorToInt(distanceTraveled).ToString();
        }
    }

     public void AddScore(int value)
    {
        score += value;
        Debug.Log("Score: " + score);
    }

    public void ResetScore()
    {
        score = 0;
        Debug.Log("Score reset to 0.");
    }

    public int GetScore()
    {
        return score;
    }
}
