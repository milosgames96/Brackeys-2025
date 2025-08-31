using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    public Slider healthBar;
    public TextMeshProUGUI ammoText;
    public GameObject deathScreenUI;
    public GameObject notificationsContainer;
    public GameObject pickUpNotificationPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathScreenUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void DisplayHealth(float health, float maxHealth)
    {
        healthBar.value = Mathf.Min(1f, health / maxHealth);
    }

    public void DisplayAmmo(float currentAmmo, float maxAmmo)
    {
        ammoText.text = (int)((currentAmmo / maxAmmo) * 100) + "%";
    }

    public void DisplayDeathScreen()
    {
        deathScreenUI.SetActive(true);
    }

    public void NotifyPickUp(Collectable collectable)
    {
        GameObject pickUpNotification = Instantiate(pickUpNotificationPrefab, notificationsContainer.transform);
        TextMeshProUGUI pickUpNotificationTest = pickUpNotification.GetComponent<TextMeshProUGUI>();
        pickUpNotificationTest.text = collectable.collectableType.ToString() + " x" + collectable.amount;
    }
}
