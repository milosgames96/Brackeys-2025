using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{

    public TextMeshProUGUI healthText;
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


    public void DisplayHealth(float health)
    {
        healthText.text = "Health: " + health;

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
