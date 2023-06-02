using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SoundManager : MonoBehaviour
{
    private static SoundManager background_music;
     void Awake()
    {
        if (background_music == null)
        {
            background_music = this;
            DontDestroyOnLoad(background_music);
        }
        else
        {
            Destroy(background_music);
        }
    }
}