using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject PauseMenuCanvas;

    void Start()
    {
        PauseMenuCanvas.SetActive(false);
        PauseController.SetPause(false);
    }

    void Update()
    {
        bool escapePressed =
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame;

        if (!escapePressed) return;

        TogglePause();
    }

    private void TogglePause()
    {
        bool newState = !PauseMenuCanvas.activeSelf;

        PauseMenuCanvas.SetActive(newState);
        PauseController.SetPause(newState);
    }

    public void ResumeButton()
    {
        PauseMenuCanvas.SetActive(false);
        PauseController.SetPause(false);
    }

    public void MainMenuButton()
    {
        PauseMenuCanvas.SetActive(false);
        PauseController.SetPause(false);

        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGameButton()
    {
        PauseMenuCanvas.SetActive(false);
        PauseController.ForceUnpause();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}