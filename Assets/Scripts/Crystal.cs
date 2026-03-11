using System;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour, IItem
{
    public static event Action<int> OnCrystalCollect;
    public int value = 5;
    public void Collect()
    {
        OnCrystalCollect?.Invoke(value);
        Destroy(gameObject);
    }
}
