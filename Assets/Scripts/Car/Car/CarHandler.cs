using System.Collections;
using System.Collections.Generic;
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
    float initialMaxVelocity = 0.001f; // Kecepatan awal

    [SerializeField]
    float finalMaxVelocity = 50f; // Kecepatan maksimum akhir

    [SerializeField]
    float velocityIncreaseRate = 0.01f; // Laju peningkatan kecepatan per detik

    float currentMaxVelocity; // Kecepatan maksimum saat ini
    float distanceTraveled = 0f; // Jarak yang telah ditempuh
    Vector3 lastPosition;

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

    void Start()
    {
        currentMaxVelocity = initialMaxVelocity;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
    // Hitung jarak yang telah ditempuh
    distanceTraveled += Vector3.Distance(transform.position, lastPosition);
    lastPosition = transform.position;

    if (rb.velocity.z < 0.5f)
    {
        rb.drag = Mathf.Lerp(rb.drag, 5f, Time.fixedDeltaTime * 2); // Tambahkan drag untuk memperlambat mobil
    }
    else
    {
        rb.drag = Mathf.Lerp(rb.drag, 0f, Time.fixedDeltaTime * 2); // Kembalikan drag ke nilai normal jika sudah kecepatan cukup
    }

    // Tingkatkan kecepatan maksimum secara bertahap
    currentMaxVelocity = Mathf.Min(currentMaxVelocity + velocityIncreaseRate * Time.fixedDeltaTime, finalMaxVelocity);

    if (input.y > 0) // Akselerasi
    {
        Accelerate();
    }
    else if (input.y < 0) // Rem
    {
        Brake();
    }
    else
    {
        // Perlambatan secara bertahap
        rb.drag = Mathf.Lerp(rb.drag, 1.5f, Time.fixedDeltaTime * 3);
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(rb.velocity.x, rb.velocity.y, 0), Time.fixedDeltaTime * 0.3f);
    }

    Steer();

    // Force the car not to go backwards
    if (rb.velocity.z <= 0)
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
    }
}

    void Accelerate()
    {
    rb.drag = 0;

    //Stay within the speed limit
    if (rb.velocity.z >= currentMaxVelocity)
        return;

    // Kurangi daya dorong di awal
    float accelerationFactor = Mathf.Lerp(1f, 10f, currentMaxVelocity / finalMaxVelocity); 

    // Menambahkan gaya untuk akselerasi
    rb.AddForce(rb.transform.forward * accelerationFactor * accelerationMultiplier * input.y);
}

    void Brake()
    {
        if (rb.velocity.z <= 0) //Cuman boleh maju gabole mundur

        {            
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        return;
        }

        // Terapkan drag untuk memperlambat mobil secara bertahap
    rb.drag = Mathf.Lerp(rb.drag, 3f, Time.fixedDeltaTime * 2); // Atur drag hingga nilai maksimum

    // Kurangi sedikit kecepatan untuk membuat perlambatan terasa halus
    rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(rb.velocity.x, rb.velocity.y, 0), Time.fixedDeltaTime * 0.5f);
        
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
}
