using System.Collections; // Tambahkan ini
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Referensi ke canvas
    public GameObject scoreCanvas;
    public GameObject menuCanvas;
    public GameObject homeMenuCanvas;

    // Referensi untuk skor dan pemain
    public CarScoreManager carScoreManager;
    public GameObject playerCar;
    public Transform startPosition;

    private bool isPaused = false;

    // Fungsi untuk mulai permainan
    public void StartGame()
    {
        homeMenuCanvas.SetActive(false);
        scoreCanvas.SetActive(true); 
    }

    // Fungsi untuk kembali ke menu
    public void GoToMenu()
    {
        scoreCanvas.SetActive(false); 
        menuCanvas.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;

        Animator[] animators = FindObjectsOfType<Animator>();
        foreach (Animator animator in animators)
        {
            animator.speed = 0; 
        }
    }

    // Fungsi untuk melanjutkan permainan
    public void ResumeGame()
    {
        menuCanvas.SetActive(false);
        scoreCanvas.SetActive(true);  
        Time.timeScale = 1;
        isPaused = false;

        Animator[] animators = FindObjectsOfType<Animator>();
        foreach (Animator animator in animators)
        {
            animator.speed = 1;
        }
    }

    // Fungsi untuk kembali ke menu utama
    public void BackToHomeMenu()
    {
        menuCanvas.SetActive(false);
        homeMenuCanvas.SetActive(true);
        Time.timeScale = 1;
        isPaused = false;
    }

    // Fungsi untuk menampilkan petunjuk cara bermain
    public void ShowHowToPlay()
    {
        Debug.Log("Show How To Play...");
    }

    // Fungsi untuk restart permainan
    public void RestartGame()
    {
        // Reset Time.timeScale ke 1 agar permainan dapat melanjutkan
        Time.timeScale = 1;

        // Restart scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Menunggu frame pertama setelah scene dimuat
        StartCoroutine(WaitForSceneLoad());
    }

    // Fungsi untuk menunggu scene selesai dimuat dan menginisialisasi ulang
    private IEnumerator WaitForSceneLoad()
    {
        // Tunggu sampai scene selesai dimuat
        yield return new WaitForEndOfFrame();

        // Pastikan objek player dan posisi awal terinisialisasi dengan benar
        if (playerCar != null && startPosition != null)
        {
            playerCar.transform.position = startPosition.position;
            playerCar.transform.rotation = startPosition.rotation;

            // Reset kecepatan mobil
            Rigidbody rb = playerCar.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Reset skor jika ada
        if (carScoreManager != null)
        {
            carScoreManager.ResetScore();
        }

        Debug.Log("Scene loaded and game restarted...");
    }
}
