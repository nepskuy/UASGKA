using TMPro;  
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public TextMeshProUGUI speedText;  
    public GameObject playerCar;     
    public float maxSpeed = 200f;     
    private bool isColliding = false;

    void Update()
    {
        
        Rigidbody carRigidbody = playerCar.GetComponent<Rigidbody>();
        if (carRigidbody != null)
        {
            
            float speed = carRigidbody.velocity.magnitude * 3.6f;  
        
            if (speed < 0.1f || isColliding)
            {
                speed = 0;
            }

           
            speed = Mathf.Clamp(speed, 0, maxSpeed);

      
            speedText.text = Mathf.FloorToInt(speed) + " km/h";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true; 
    }

    
    private void OnCollisionExit(Collision collision)
    {
        isColliding = false; 
    }
}
