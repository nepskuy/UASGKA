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
        originalObject.SetActive(false);

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
    }

    public bool HandleCollision(Collision collision, Vector3 velocity)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            collisionCount++;
            Debug.Log($"Collision Count: {collisionCount}");

            if (collisionCount >= collisionThreshold)
            {
                Explode(velocity * 45);
                return true; // Menandakan mobil telah meledak
            }
        }

        return false; // Mobil belum meledak
    }
}
