using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public GameObject comicPanel;
    public Button goButton;

    void Start()
    {
        // Main menu is visible and credits are hidden on start
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (comicPanel != null) comicPanel.SetActive(false);
        goButton.onClick.AddListener(GoToCharacterSelect);
    }

    public void PlayGame(string sceneName)
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(false);
        comicPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void HideCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void GoToCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
