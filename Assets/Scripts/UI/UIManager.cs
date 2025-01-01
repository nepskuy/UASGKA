using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject scoreCanvas;
    public GameObject menuCanvas;
    public GameObject homeMenuCanvas;
    public GameObject gameOverCanvas; // Game Over Canvas
    public GameObject creditCanvas; // Credit Canvas

    public CarScoreManager carScoreManager;
    public GameObject playerCar;
    public Transform startPosition;
    public GameObject howToPlayCanvas;

    public AudioSource carEngineAudioSource;

    private bool isPaused = false;

    private void Start()
    {
        howToPlayCanvas.SetActive(false);
        gameOverCanvas.SetActive(false); // Nonaktifkan Game Over Canvas di awal
        menuCanvas.SetActive(false);    // Nonaktifkan Menu Canvas di awal
        creditCanvas.SetActive(false);  // Nonaktifkan Credit Canvas di awal
    }

    public void StartGame()
    {
        homeMenuCanvas.SetActive(false);
        scoreCanvas.SetActive(true);
    }

    public void GoToMenu()
    {
        if (carEngineAudioSource != null)
        {
            Debug.Log("Pausing engine sound...");
            carEngineAudioSource.Pause();
        }
        else
        {
            Debug.Log("carEngineAudioSource is null!");
        }

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

        if (carEngineAudioSource != null)
        {
            carEngineAudioSource.UnPause();
        }
    }

    public void BackToHomeMenu()
    {
        menuCanvas.SetActive(false);
        homeMenuCanvas.SetActive(true);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void ShowHowToPlay()
    {
        homeMenuCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);

        Debug.Log("Show How To Play...");
    }

    public void BackFromHowToPlay()
    {
        howToPlayCanvas.SetActive(false);
        menuCanvas.SetActive(true);

        Time.timeScale = 0;
        isPaused = true;

        Animator[] animators = FindObjectsOfType<Animator>();
        foreach (Animator animator in animators)
        {
            animator.speed = 0;
        }

        Debug.Log("Returned from How To Play and opened Menu...");
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        StartCoroutine(WaitForSceneLoad());
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForEndOfFrame();

        if (playerCar != null && startPosition != null)
        {
            playerCar.transform.position = startPosition.position;
            playerCar.transform.rotation = startPosition.rotation;

            Rigidbody rb = playerCar.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (carScoreManager != null)
        {
            carScoreManager.ResetScore();
        }

        gameOverCanvas.SetActive(false); // Nonaktifkan Game Over Canvas saat restart
        menuCanvas.SetActive(false);    // Nonaktifkan Menu Canvas saat restart
        Debug.Log("Scene loaded and game restarted...");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting the game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowGameOver()
    {
        gameOverCanvas.SetActive(true); // Tampilkan Game Over Canvas
        menuCanvas.SetActive(true);    // Tampilkan Menu Canvas
        scoreCanvas.SetActive(false);  // Sembunyikan score canvas (opsional)

        Debug.Log("Game Over!");
    }

    public void ShowCredits()
    {
        homeMenuCanvas.SetActive(false); // Sembunyikan Home Menu
        creditCanvas.SetActive(true);   // Tampilkan Credit Canvas

        Debug.Log("Credits shown.");
    }

    public void CloseCredits()
    {
        creditCanvas.SetActive(false);  // Sembunyikan Credit Canvas
        homeMenuCanvas.SetActive(true); // Kembali ke Home Menu

        Debug.Log("Credits closed.");
    }
}
