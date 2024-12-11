using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isScoreRunning = true; // Status apakah skor sedang berjalan

    // Contoh fungsi untuk menghentikan skor
    public void StopScore()
    {
        isScoreRunning = false;
        Debug.Log("Score has been stopped.");
    }

    // Contoh fungsi untuk memulai skor
    public void StartScore()
    {
        isScoreRunning = true;
        Debug.Log("Score is running.");
    }
}
