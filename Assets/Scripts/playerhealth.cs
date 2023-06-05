using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerhealth : MonoBehaviour
{
    public Slider player;
    
    public void setHealth(int health)
    {
        player.value = health;
    }
    public void setMaxHealth(int max)
    {
        player.maxValue = max;
    }
}
