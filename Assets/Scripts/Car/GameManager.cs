using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isScoreRunning = true; 


    public void StopScore()
    {
        isScoreRunning = false;
        Debug.Log("Score has been stopped.");
    }

    public void StartScore()
    {
        isScoreRunning = true;
        Debug.Log("Score is running.");
    }
}
