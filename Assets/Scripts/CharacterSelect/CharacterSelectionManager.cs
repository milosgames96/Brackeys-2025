using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{

    public static PlayerProfile selectedPlayerProfile;

    public Button firstButton;
    public Button secondButton;
    public PlayerProfile firstPlayerProfile;
    public PlayerProfile secondPlayerProfile;

    public GameObject first;
    public GameObject second;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstButton.onClick.AddListener(SelectFirst);
        secondButton.onClick.AddListener(SelectSecond);
    }

 // Update is called once per frame
    void Update()
    {
        first.transform.Rotate(Vector3.up * 45 * Time.deltaTime, Space.World);
        second.transform.Rotate(Vector3.up * 45 * Time.deltaTime, Space.World);
    }

    private void SelectFirst()
    {
        selectedPlayerProfile = firstPlayerProfile;
        StartGame();
    }

    private void SelectSecond()
    {
        selectedPlayerProfile = secondPlayerProfile;
        StartGame();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
