using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossWorkaround : MonoBehaviour
{
    public Image black;
    public GameObject endMenu;
    public Button restartButton;
    public Button exitButton;
    public AudioClip endSound;
    public AudioSource audioSource;
    private bool isDone;
    private float fadeDuration = 1.2f;

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player" || other.tag == "PlayerTrigger") && !isDone)
        {
            isDone = true;
            black.gameObject.SetActive(true);
            StartCoroutine(FadeToBlack());
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(endSound);
            }
        }
    }

    public IEnumerator FadeToBlack()
    {
        float timer = 0f;
        Color c = black.color;
        c.a = 0f;
        black.color = c;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            black.color = c;
            yield return null;
        }

        if (endMenu != null)
        {
            endMenu.SetActive(true);
            black.gameObject.SetActive(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}