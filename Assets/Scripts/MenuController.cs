using UnityEngine;
using UnityEngine.InputSystem;

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

        bool newState = !PauseMenuCanvas.activeSelf;

        PauseMenuCanvas.SetActive(newState);
        PauseController.SetPause(newState);
    }
}