using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    // Start is called before the first frame update
   
    float accelerationMultiplier = 3;
    float breaksMultiplier = 15;
    float steeringMultiplier = 5;

    Vector2 input = Vector2.zero;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (input.y > 0) //Acceleration
            Accelerate();
        else
            rb.drag = 0.2f;

        if (input.y < 0) //Breaks
            Brake();

        Steer();
    }

    void Accelerate()
    {
        rb.drag = 0;

        rb.AddForce(rb.transform.forward * 10 * accelerationMultiplier * input.y);
    }

    void Brake()
    {
        if (rb.velocity.z <= 0) //Cuman boleh maju gabole mundur
            return;
        
        rb.AddForce(rb.transform.forward * 10 * breaksMultiplier * input.y);
    }


    void Steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            rb.AddForce(rb.transform.right * steeringMultiplier * input.x);
        }
    }

    public void SetInput(Vector2 inputVector)
    {
        inputVector.Normalize();

        input = inputVector;

    }
}
