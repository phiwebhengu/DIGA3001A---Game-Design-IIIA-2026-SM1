using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class HoldToLoadLevel : MonoBehaviour
{
    [Header("Hold Settings")]
    public float holdDuration = 1f;
    public Image fillcircle;

    [Header("Prompt UI")]
    public TMP_Text holdPromptText;
    public string promptMessage = "Hold E to move to next level";

    private float holdTimer = 0f;
    private bool isHolding = false;

    public static event Action OnHoldComplete;

    void Start()
    {
        ResetHold();

        if (holdPromptText != null)
            holdPromptText.text = promptMessage;
    }

    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            if (fillcircle != null)
                fillcircle.fillAmount = holdTimer / holdDuration;

            if (holdTimer >= holdDuration)
            {
                OnHoldComplete?.Invoke();
                ResetHold();
            }
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
        }
        else if (context.canceled)
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;

        if (fillcircle != null)
            fillcircle.fillAmount = 0f;
    }
}