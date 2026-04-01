using System;
using UnityEngine;

public class Crystal : MonoBehaviour, IItem
{
    public static event Action<int> OnCrystalCollect;
    public int value = 5;

    private bool collected = false;

    void OnEnable()
    {
        GameController.OnReset += ResetCrystal;
    }

    void OnDisable()
    {
        GameController.OnReset -= ResetCrystal;
    }

    public void Collect()
    {
        if (collected)
            return;

        collected = true;
        OnCrystalCollect?.Invoke(value);
        gameObject.SetActive(false);
    }

    private void ResetCrystal()
    {
        collected = false;
        gameObject.SetActive(true);
    }
}