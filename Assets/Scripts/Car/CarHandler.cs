using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    private bool isPlayer = false;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Transform gameModel;

    [SerializeField]
    private MeshRenderer[] carMeshesRender;

    [SerializeField]
    private ExplodeHandler explodeHandler;

    [Header("SFX")]
    [SerializeField]
    private AudioSource carEngineAS;

    [SerializeField]
    private AnimationCurve carPitchAnimationCurve;

    [SerializeField]
    private AudioSource carCrashAS;

    [Header("Game Management")]
    [SerializeField]
    private MonoBehaviour cameraController;

    [SerializeField]
    private GameManager gameManager;

    [Header("Life Count UI")]
    [SerializeField]
    private RawImage[] lifeImages; // Array untuk menyimpan 3 gambar life count

    private float maxSteerVelocity = 2f;
    private float maxForwardVelocity = 30f;
    private float accelerationMultiplier = 0.5f;
    private float steeringMultiplier = 5f;

    private Vector2 input = Vector2.zero;

    private int collisionCount = 0; // Hitungan tabrakan
    private bool isExploded = false;
    private bool isGameStarted = false;
    private bool isInvincible = false; // Status tidak bisa ditabrak

    public void StartGame()
    {
        isGameStarted = true;
        if (isGameStarted)
            carEngineAS.Play();
    }

    void Update()
    {
        if (isExploded)
        {
            FadeOutCarAudio();
            return;
        }

        UpdateCarAudio();
    }

    private void FixedUpdate()
    {
        if (!isGameStarted || isExploded) return;

        Accelerate();
        Steer();
    }

    void Accelerate()
    {
        rb.drag = 0;

        if (rb.velocity.z < maxForwardVelocity)
        {
            rb.AddForce(rb.transform.forward * accelerationMultiplier, ForceMode.Acceleration);
        }
    }

    void Steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            float steeringForce = (steeringMultiplier * input.x) / 2.0f;
            rb.AddForce(rb.transform.right * steeringForce);

            float normalizedX = Mathf.Clamp(rb.velocity.x, -maxSteerVelocity / 2.0f, maxSteerVelocity / 2.0f);
            rb.velocity = new Vector3(normalizedX, rb.velocity.y, rb.velocity.z);
        }
    }

    void UpdateCarAudio()
    {
        if (!isGameStarted)
            return;

        carEngineAS.pitch = carPitchAnimationCurve.Evaluate(rb.velocity.z / maxForwardVelocity);
    }

    void FadeOutCarAudio()
    {
        if (!isPlayer) return;

        carEngineAS.volume = Mathf.Lerp(carEngineAS.volume, 0, Time.deltaTime * 10);
    }

    public void SetInput(Vector2 inputVector)
    {
        input = inputVector;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isInvincible) return;

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            collisionCount++;

            UpdateLifeUI();

            if (collisionCount < 3)
            {
                StartCoroutine(HandleBlinkEffect());
            }
            else if (collisionCount == 3)
            {
                ExplodeCar();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthCapsule"))
        {
            AddHealth(); // Tambahkan health
            Destroy(other.gameObject); // Hapus capsule setelah diambil
        }
    }

    private void UpdateLifeUI()
    {
        if (collisionCount - 1 < lifeImages.Length)
        {
            lifeImages[collisionCount - 1].enabled = false;
        }
    }

    private void AddHealth()
    {
        // Tambahkan health jika belum penuh
        if (collisionCount > 0)
        {
            collisionCount--;
            if (collisionCount < lifeImages.Length)
            {
                lifeImages[collisionCount].enabled = true;
            }
        }
    }

    private IEnumerator HandleBlinkEffect()
    {
        Debug.Log($"Collision {collisionCount}. Blinker activated.");

        isInvincible = true;

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        float originalDrag = rb.drag;
        rb.drag = 10f;

        float blinkDuration = 3f;
        float blinkInterval = 0.2f;
        float timer = 0;

        while (timer < blinkDuration)
        {
            foreach (var mesh in carMeshesRender)
            {
                mesh.enabled = !mesh.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }

        foreach (var mesh in carMeshesRender)
        {
            mesh.enabled = true;
        }

        rb.drag = originalDrag;

        isInvincible = false;
        Debug.Log("Blinker deactivated. Car is vulnerable again.");
    }

    private void ExplodeCar()
    {
        Debug.Log("Car exploded!");
        isExploded = true;

        Vector3 velocity = rb.velocity;
        explodeHandler.Explode(velocity * 45);

        if (cameraController != null)
        {
            var virtualCamera = cameraController.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                Vector3 cameraPosition = virtualCamera.transform.position;
                virtualCamera.Follow = null;
                virtualCamera.LookAt = null;

                virtualCamera.transform.position = cameraPosition;
                virtualCamera.transform.rotation = Quaternion.identity;

                virtualCamera.enabled = false;
            }
        }

        FadeOutCarAudio();
        carEngineAS.Stop();

        carCrashAS.Play();

        FindObjectOfType<CarScoreManager>().GameOver();
    }
}
