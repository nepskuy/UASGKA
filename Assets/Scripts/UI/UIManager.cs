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
    public GameObject howToPlayCanvas;

    // Referensi AudioSource untuk suara mesin mobil
    public AudioSource carEngineAudioSource; // Pastikan ini dihubungkan di Inspector

    private bool isPaused = false;

    private void Start()
    {
        howToPlayCanvas.SetActive(false); // Pastikan canvas HowToPlay tidak aktif di awal
    }

    // Fungsi untuk mulai permainan
    public void StartGame()
    {
        homeMenuCanvas.SetActive(false);
        scoreCanvas.SetActive(true); 
    }

    // Fungsi untuk kembali ke menu
    public void GoToMenu()
    {
            // Matikan suara mesin mobil sebelum mengubah tampilan menu
        if (carEngineAudioSource != null)
        {
            Debug.Log("Pausing engine sound...");  // Debugging untuk memastikan suara dipause
            carEngineAudioSource.Pause(); // Pause suara mesin
        }
        else
        {
            Debug.Log("carEngineAudioSource is null!");  // Jika carEngineAudioSource tidak terhubung
        }

        // Lanjutkan dengan mengubah tampilan menu
        scoreCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;

        Animator[] animators = FindObjectsOfType<Animator>();
        foreach (Animator animator in animators)
        {
            animator.speed = 0; // Hentikan animasi
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

        // Unpause suara mesin setelah resume
        if (carEngineAudioSource != null)
        {
            carEngineAudioSource.UnPause(); // Unpause suara mesin
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
        // Menonaktifkan canvas lainnya dan mengaktifkan HowToPlayCanvas
        homeMenuCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);

        Debug.Log("Show How To Play...");
    }

    public void BackFromHowToPlay()
    {
        // Menonaktifkan HowToPlayCanvas dan mengaktifkan MenuCanvas
        howToPlayCanvas.SetActive(false);
        menuCanvas.SetActive(true);

        // Menghentikan waktu permainan (Pause)
        Time.timeScale = 0;
        isPaused = true;

        // Menghentikan semua animasi
        Animator[] animators = FindObjectsOfType<Animator>();
        foreach (Animator animator in animators)
        {
            animator.speed = 0;
        }

        Debug.Log("Returned from How To Play and opened Menu...");
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

    // Fungsi untuk keluar dari aplikasi
    public void ExitGame()
    {
        Debug.Log("Exiting the game...");

        // Jika sedang di editor, tidak bisa keluar aplikasi, maka tampilkan log
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Jika di build aplikasi, keluar dari aplikasi
            Application.Quit();
        #endif
    }
}
