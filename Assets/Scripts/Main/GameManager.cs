using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    // Singleton instance
    public static GameManager instance;

    public TextMeshProUGUI killCountText;
    private int enemiesKilled = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateKillCountText();
    }

    public void AddKill()
    {
        enemiesKilled++;
        UpdateKillCountText();
    }

    private void UpdateKillCountText()
    {
        killCountText.text = "KILLS: " + enemiesKilled;
    }
}