using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHUD : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    public void Init(int maxhealth)
    {
        healthSlider.maxValue = maxhealth;
        healthSlider.value = maxhealth; 
    }

    public void OnHealthChanged(int health)
    {
        healthSlider.value = health;
    }
}
