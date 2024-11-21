using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform gameModel;
    // Start is called before the first frame update
   

   //max values
   float maxSteerVelocity = 2;
   float maxForwardVelocity = 30;

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
        //Rotate car model when "turning"
        gameModel.transform.rotation = Quaternion.Euler(0, rb.velocity.x * 5, 0);
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

        //Force the car not to go backwards
        if (rb.velocity.z <= 0)
        rb.velocity = Vector3.zero;
    }

    void Accelerate()
    {
        rb.drag = 0;

        //Stay within the speed limit
        if (rb.velocity.z >= maxForwardVelocity)
            return;

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
            //Move the car sideways
            float speedBaseSteerLimit = rb.velocity.z / 5.0f;
            speedBaseSteerLimit = Mathf.Clamp01(speedBaseSteerLimit);

            rb.AddForce(rb.transform.right * steeringMultiplier * input.x * speedBaseSteerLimit);

            //Normalize the X Velocity
            float normalizedX = rb.velocity.x /maxSteerVelocity;
            
            //Ensure that we dont allow it to get bigger than 1 in magnitued
            normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f) ;

            //Make sure we stay within the turn speed limit
            rb.velocity = new Vector3(normalizedX * maxSteerVelocity, 0, rb.velocity.z);  
            }
            else
            {
                //Auto center car
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, rb.velocity.z), Time.fixedDeltaTime * 3);
            }
    }

    public void SetInput(Vector2 inputVector)
    {
        inputVector.Normalize();

        input = inputVector;

    }
}
