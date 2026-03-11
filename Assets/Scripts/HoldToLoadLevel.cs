using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class HoldToLoadLevel : MonoBehaviour
{
    public float holdDuration = 1f; // Time required to hold before loading the level
    public Image fillcircle; // UI element to show hold progress

    private float holdTimer = 0f;
    private bool isHolding = false;

    public static event Action OnHoldComplete;

    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
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
        fillcircle.fillAmount = 0f;
    }
}
