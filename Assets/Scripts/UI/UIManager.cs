using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject scoreCanvas;
    public GameObject menuCanvas;
    public GameObject homeMenuCanvas;
    public GameObject gameOverCanvas; 
    public GameObject creditCanvas; 

    public CarScoreManager carScoreManager;
    public GameObject playerCar;
    public Transform startPosition;
    public GameObject howToPlayCanvas;

    public AudioSource carEngineAudioSource;

    private bool isPaused = false;

    private void Start()
    {
        howToPlayCanvas.SetActive(false);
        gameOverCanvas.SetActive(false); 
        menuCanvas.SetActive(false);    
        creditCanvas.SetActive(false);
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

        gameOverCanvas.SetActive(false); 
        menuCanvas.SetActive(false);   
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
        gameOverCanvas.SetActive(true); 
        menuCanvas.SetActive(false);  
        scoreCanvas.SetActive(false); 

        Debug.Log("Game Over!");
    }

    public void ShowCredits()
    {
        homeMenuCanvas.SetActive(false); 
        creditCanvas.SetActive(true);   

        Debug.Log("Credits shown.");
    }

    public void CloseCredits()
    {
        creditCanvas.SetActive(false);  
        homeMenuCanvas.SetActive(true); 

        Debug.Log("Credits closed.");
    }

    public void PlayAgain()
    {
        Debug.Log("Play Again button clicked...");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(WaitForSceneLoad());
    }
}
