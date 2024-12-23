using System.Collections;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    private bool isPlayer = false; 

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform gameModel;

    [SerializeField]
    MeshRenderer[] carMeshesRender;

    [SerializeField]
    ExplodeHandler explodeHandler;

    [Header("SFX")]
    [SerializeField]
    AudioSource carEngineAS;

    [SerializeField]
    AnimationCurve carPitchAnimationCurve;

    [SerializeField]
    AudioSource carCrashAS;

    [Header("Game Management")]
    [SerializeField]
    private MonoBehaviour cameraController; 

    [SerializeField]
    private GameManager gameManager;

    
    float maxSteerVelocity = 2f;
    float maxForwardVelocity = 30f;
    float accelerationMultiplier = 0.5f;
    float steeringMultiplier = 5f;
    float carMaxSpeedPercentage = 0;

    Vector2 input = Vector2.zero;

    
    int _emissionColor = Shader.PropertyToID("_EmissionColor");
    Color emissiveColor = Color.white;
    float emissiveColorMultiplier = 0f;

    
    bool isExploded = false;
    bool isGameStarted = false; 

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

    
        gameModel.transform.rotation = Quaternion.Euler(0, rb.velocity.x * 5, 0);

    
        if (carMeshesRender != null)
        {
            foreach (var carMesh in carMeshesRender)
            {
                carMesh.material.SetColor(_emissionColor, emissiveColor * emissiveColorMultiplier);
            }
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
       
        float speedBaseSteerLimit = rb.velocity.z / maxForwardVelocity;
        speedBaseSteerLimit = Mathf.Clamp01(speedBaseSteerLimit);

        float steeringForce = steeringMultiplier * input.x * speedBaseSteerLimit;
        rb.AddForce(rb.transform.right * steeringForce);

        float normalizedX = rb.velocity.x / maxSteerVelocity;
        normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);
        rb.velocity = new Vector3(normalizedX * maxSteerVelocity, 0, rb.velocity.z);
    }
    else
    {
        
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, rb.velocity.z), Time.fixedDeltaTime * 2); 
    }
}


    void UpdateCarAudio()
    {
        if (!isGameStarted)
            return;

        carMaxSpeedPercentage = rb.velocity.z / maxForwardVelocity;

        carEngineAS.pitch = carPitchAnimationCurve.Evaluate(carMaxSpeedPercentage);
    }

    void FadeOutCarAudio()
    {
        if (!isPlayer)
        return;

        carEngineAS.volume = Mathf.Lerp(carEngineAS.volume, 0, Time.deltaTime * 10);
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

    
    carCrashAS.volume = Mathf.Clamp(carMaxSpeedPercentage, 0.25f, 1.0f);
    carCrashAS.pitch = Mathf.Clamp(carMaxSpeedPercentage, 0.3f, 1.0f);
    carCrashAS.Play();

    
    FindObjectOfType<CarScoreManager>().GameOver();

    StartCoroutine(SlowDownTimeCO());
    }
}
