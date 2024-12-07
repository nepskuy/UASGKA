using System.Collections;
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

    // Maksimal nilai
    float maxSteerVelocity = 2f;
    float maxForwardVelocity = 30f;
    float accelerationMultiplier = 0.5f; // Akselerasi bertahap
    float steeringMultiplier = 5f;

    Vector2 input = Vector2.zero;

    // Emissive Properties
    int _emissionColor = Shader.PropertyToID("_EmissionColor");
    Color emissiveColor = Color.white;
    float emissiveColorMultiplier = 0f;

    // Kondisi permainan
    bool isExploded = false;
    bool isGameStarted = false; // Menunggu pemain menekan spasi
        
    public void StartGame()
    {
        isGameStarted = true; // Mulai permainan
        // Kamu bisa tambahkan animasi atau efek lainnya jika perlu
    }
    void Update()
    {
        // Jika permainan belum dimulai, cek input spasi
        if (!isGameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            isGameStarted = true; // Mulai permainan
        }

        if (isExploded || !isGameStarted) return;

        // Rotate car model saat belok
        gameModel.transform.rotation = Quaternion.Euler(0, rb.velocity.x * 5, 0);

        // Update emissive properties
        if (carMeshesRender != null)
        {
            foreach (var carMesh in carMeshesRender)
            {
                carMesh.material.SetColor(_emissionColor, emissiveColor * emissiveColorMultiplier);
            }
        }
    }

    private void FixedUpdate()
    {
        // Tidak melakukan apa-apa jika permainan belum dimulai
        if (!isGameStarted || isExploded) return;

        // Akselerasi otomatis
        Accelerate();

        // Steering (Belok)
        Steer();
    }

    void Accelerate()
    {
        rb.drag = 0; // Hilangkan drag

        // Pastikan kecepatan tidak melebihi batas maksimum
        if (rb.velocity.z < maxForwardVelocity)
        {
            rb.AddForce(rb.transform.forward * accelerationMultiplier, ForceMode.Acceleration);
        }
    }

    void Steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            // Gerakkan mobil ke samping
            float speedBaseSteerLimit = rb.velocity.z / maxForwardVelocity;
            speedBaseSteerLimit = Mathf.Clamp01(speedBaseSteerLimit);

            rb.AddForce(rb.transform.right * steeringMultiplier * input.x * speedBaseSteerLimit);

            // Normalisasi kecepatan sumbu X
            float normalizedX = rb.velocity.x / maxSteerVelocity;

            // Pastikan nilai tidak lebih dari -1 hingga 1
            normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);

            // Pastikan kecepatan belok tetap dalam batas
            rb.velocity = new Vector3(normalizedX * maxSteerVelocity, 0, rb.velocity.z);
        }
        else
        {
            // Otomatis luruskan mobil
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
            Time.timeScale -= Time.deltaTime * 2;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while (Time.timeScale < 1.0f)
        {
            Time.timeScale += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit {collision.collider.name}");

        Vector3 velocity = rb.velocity;
        explodeHandler.Explode(velocity * 45);

        isExploded = true;

        StartCoroutine(SlowDownTimeCO());
    }
}
