using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Referensi ke canvas
    public GameObject scoreCanvas;
    public GameObject menuCanvas;   // Menu canvas yang berisi tombol restart, how to play, dan exit
    public GameObject homeMenuCanvas;

    // Referensi untuk skor dan pemain
    public CarScoreManager carScoreManager; // Script untuk mengatur skor
    public GameObject playerCar;     // Objek mobil pemain
    public Transform startPosition;  // Posisi awal mobil (drag dari editor)

    private bool isPaused = false; // Status apakah game sedang pause atau tidak

    public void StartGame()
    {
        // Sembunyikan Home Menu Canvas
        homeMenuCanvas.SetActive(false);

        // Tampilkan Gameplay Canvas atau Score Canvas, tergantung pada struktur game kamu
        scoreCanvas.SetActive(true); // Misalnya kamu ingin menampilkan ScoreCanvas sebagai permulaan permainan

        // Mulai permainan, bisa menambahkan logika inisialisasi permainan di sini
    }

    // Fungsi untuk pergi ke MenuCanvas dari ScoreCanvas (dan pause game)
    public void GoToMenu()
    {
        scoreCanvas.SetActive(false);  // Sembunyikan Score Canvas
        menuCanvas.SetActive(true);    // Tampilkan Menu Canvas

        // Pause game dengan menghentikan waktu
        Time.timeScale = 0; // Menghentikan semua pergerakan di game
        isPaused = true;
    }

    // Fungsi untuk melanjutkan permainan (resume)
    public void ResumeGame()
    {
        menuCanvas.SetActive(false);   // Sembunyikan Menu Canvas
        scoreCanvas.SetActive(true);  // Kembali ke Score Canvas (atau gameplay canvas)

        // Lanjutkan game dengan mengembalikan waktu
        Time.timeScale = 1; // Memulai kembali semua pergerakan
        isPaused = false;
    }

    // Fungsi untuk kembali ke HomeMenu dari MenuCanvas
    public void BackToHomeMenu()
    {
        menuCanvas.SetActive(false);    // Sembunyikan Menu Canvas
        homeMenuCanvas.SetActive(true); // Tampilkan Home Menu Canvas

        // Pastikan waktu tetap berjalan normal
        Time.timeScale = 1;
        isPaused = false;
    }

    // Fungsi untuk menampilkan layar "How To Play"
    public void ShowHowToPlay()
    {
        Debug.Log("Show How To Play...");
    }

    // Fungsi untuk restart game
    public void RestartGame()
    {
        // Aktifkan kembali ScoreCanvas
        scoreCanvas.SetActive(true);

        // Sembunyikan MenuCanvas
        menuCanvas.SetActive(false);

        // Reset status pause
        Time.timeScale = 1;
        isPaused = false;

        // Reset skor
        if (carScoreManager != null)
        {
            carScoreManager.ResetScore();
        }

        // Reset posisi mobil
        if (playerCar != null && startPosition != null)
        {
            playerCar.transform.position = startPosition.position; // Atur posisi awal
            playerCar.transform.rotation = startPosition.rotation; // Atur rotasi awal
        }

        Debug.Log("Game restarted...");
    }

    private void Update()
    {
        // Toggle pause/resume saat pemain menekan Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                GoToMenu();
            }
        }
    }
}
