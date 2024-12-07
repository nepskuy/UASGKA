using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Rendering;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform gameModel;

    [SerializeField]
    MeshRenderer[] carMeshesRender;

    [SerializeField]
    ExplodeHandler explodeHandler;

    //max values
    float maxSteerVelocity = 2;
    float maxForwardVelocity = 30;

    float accelerationMultiplier = 3;
    float breaksMultiplier = 15;
    float steeringMultiplier = 5;

    Vector2 input = Vector2.zero;

    //Emissive Properties
    int _emissionColor = Shader.PropertyToID("_EmissionColor");
    Color emissiveColor = Color.white;
    float emissiveColorMultiplier = 0f;

    // Explode state
    bool isExploded = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if(isExploded)
        return;

        //Rotate car model when "turning"
        gameModel.transform.rotation = Quaternion.Euler(0, rb.velocity.x * 5, 0);

        if (carMeshesRender != null)
        {
            float desiredCarEmissiveColorMultiplier = 0f;

            if (input.y < 0)
                desiredCarEmissiveColorMultiplier = 4.0f;

            emissiveColorMultiplier = Mathf.Lerp(emissiveColorMultiplier, desiredCarEmissiveColorMultiplier, Time.deltaTime * 4);

            foreach (var carMesh in carMeshesRender)
            {
                carMesh.material.SetColor(_emissionColor, emissiveColor * emissiveColorMultiplier);
            }
        }

        // Atur emissive color lampu belakang berdasarkan input rem
        Color brakeLightColor = input.y < 0 ? Color.red : Color.black;
        foreach (var carMesh in carMeshesRender)
        {
            carMesh.material.SetColor(_emissionColor, brakeLightColor);
        }
    }

    private void FixedUpdate()
    {

        //is exploded
        if (isExploded)
        {
            //Apply Drag
            rb.drag = rb.velocity.z * 0.1f;
            rb.drag = Mathf.Clamp(rb.drag, 1.5f, 10);

            //Move towards after a the car has exploded
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(0, 0, transform.position.z), Time.deltaTime * 0.5f));

            return;
        }

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
            float normalizedX = rb.velocity.x / maxSteerVelocity;

            //Ensure that we dont allow it to get bigger than 1 in magnitude
            normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);

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

    IEnumerator SlowDownTimeCO()
    {
        while (Time.timeScale > 0.2f)
        {
            Time.timeScale -= Time.deltaTime * 2 ;

            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        while (Time.timeScale <= 1.0f)
        {
            Time.timeScale += Time.deltaTime;

            yield return null;
        }

        Time.timeScale = 1.0f;
    }

//  Events
    private void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log($"Hit {collision.collider.name}");

        Vector3 velocity = rb.velocity;
        explodeHandler.Explode(velocity * 45);

        isExploded = true;

        StartCoroutine(SlowDownTimeCO());
    }
}
