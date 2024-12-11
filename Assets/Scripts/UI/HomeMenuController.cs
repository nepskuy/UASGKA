using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Stage"); 
    }
}
