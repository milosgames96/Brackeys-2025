using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Audio")]
    public AudioClip shotSound;
    private AudioSource audioSource;

    [Header("DEV Tweaks")]
    public float fireRate = 10f;
    private float nextFireTime = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Audio Source not found.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && Time.timeScale!=0)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (shotSound != null)
        {
            audioSource.PlayOneShot(shotSound);
        }

        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }
}