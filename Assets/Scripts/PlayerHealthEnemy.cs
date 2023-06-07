using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class PlayerHealthEnemy : MonoBehaviour
{
    private Slider m_PlayerSlider;

    public void SetHealth(int health)
    {
        m_PlayerSlider.value = health;
    }
    public void SetMaxHealth(int max)
    {
        m_PlayerSlider = GetComponent<Slider>();
        m_PlayerSlider.maxValue = max;
    }
}
