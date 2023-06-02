using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sound : MonoBehaviour
{
    [SerializeField] Slider Slider;
    void Start()
    {
        if (PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetFloat("musicvolume", 1);
            load();
        }
        else
        {
            load();
        }
    }

    public void Changevolume()
    {
        AudioListener.volume = Slider.value;
        save();
    }
    private void load()
    {
        Slider.value = PlayerPrefs.GetFloat("musicvolume");
    }
    private void save()
    {
        PlayerPrefs.SetFloat("musicvolume", Slider.value);

    }
}
