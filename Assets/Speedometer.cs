using TMPro;  // Pastikan baris ini ada
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public TextMeshProUGUI speedText;  // TMP Text untuk menampilkan kecepatan
    public GameObject playerCar;      // Mobil pemain
    public float maxSpeed = 200f;     // Kecepatan maksimum yang diinginkan
    private bool isColliding = false; // Variabel untuk mendeteksi tabrakan

    void Update()
    {
        // Mendapatkan komponen Rigidbody dari player car
        Rigidbody carRigidbody = playerCar.GetComponent<Rigidbody>();
        if (carRigidbody != null)
        {
            // Menghitung kecepatan mobil dalam km/h
            float speed = carRigidbody.velocity.magnitude * 3.6f;  // Konversi m/s ke km/h

            // Jika mobil sedang berhenti atau terjadi tabrakan, set kecepatan menjadi 0
            if (speed < 0.1f || isColliding) // Jika kecepatan sangat kecil atau sedang tabrakan
            {
                speed = 0;
            }

            // Membatasi kecepatan
            speed = Mathf.Clamp(speed, 0, maxSpeed);

            // Menampilkan kecepatan di TextMeshPro
            speedText.text = Mathf.FloorToInt(speed) + " km/h";
        }
    }

    // Fungsi untuk mendeteksi tabrakan
    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true; // Menandakan bahwa mobil sedang bertabrakan
    }

    // Fungsi untuk mendeteksi tabrakan berhenti
    private void OnCollisionExit(Collision collision)
    {
        isColliding = false; // Mengatur kembali ke kondisi tidak bertabrakan
    }
}
