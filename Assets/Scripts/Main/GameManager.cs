using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // Singleton instance
    public static GameManager instance;
    public GameObject escapeMenuUI;

    private bool isPaused = false;
    private int enemiesKilled = 0;

    public Button exitButton;
    public Button restartButton;
    public Button resumeButton;
    public Button restartPausedButton;
    public Button exitPausedButton;
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

        // FPS limit
        Application.targetFrameRate = 150;
    }

    void Start()
    {
        escapeMenuUI.SetActive(false);
        exitButton.onClick.AddListener(ExitGame);
        restartButton.onClick.AddListener(RestartGame);
        restartPausedButton.onClick.AddListener(RestartGame);
        exitPausedButton.onClick.AddListener(ExitGame);
        resumeButton.onClick.AddListener(ResumeGame);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !exitButton.IsActive())
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        escapeMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        escapeMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CharacterSelect");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void AddKill()
    {
        enemiesKilled++;
    }
}