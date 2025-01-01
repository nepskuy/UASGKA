using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject originalObject;

    [SerializeField]
    private GameObject model;

    [SerializeField]
    private int collisionThreshold = 3; // Jumlah tabrakan sebelum meledak

    private Rigidbody[] rigidbodies;
    private int collisionCount = 0; // Hitungan tabrakan

    private void Awake()
    {
        rigidbodies = model.GetComponentsInChildren<Rigidbody>(true);
    }

    public void Explode(Vector3 externalForce)
    {
        // Matikan objek asli
        originalObject.SetActive(false);

        // Aktifkan potongan-potongan model yang meledak
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.transform.parent = null;
            rb.GetComponent<MeshCollider>().enabled = true;

            rb.gameObject.SetActive(true);
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.AddForce(Vector3.up * 200 + externalForce, ForceMode.Force);
            rb.AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);
        }

        // Panggil Game Over dari UIManager
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            Debug.Log("Game Over triggered by ExplodeHandler.");
            uiManager.ShowGameOver();
        }
        else
        {
            Debug.LogError("UIManager not found! Game Over cannot be triggered.");
        }
    }

    public bool HandleCollision(Collision collision, Vector3 velocity)
    {
        // Periksa jika tabrakan terjadi dengan objek bertag "Obstacle"
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            collisionCount++;
            Debug.Log($"Collision Count: {collisionCount}");

            // Jika sudah mencapai batas tabrakan, lakukan ledakan
            if (collisionCount >= collisionThreshold)
            {
                Explode(velocity * 45);
                return true; // Menandakan mobil telah meledak
            }
        }

        return false; // Mobil belum meledak
    }
}
