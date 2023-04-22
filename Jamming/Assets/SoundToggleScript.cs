using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundToggleScript : MonoBehaviour
{
    public void SoundToggle(bool soundOn)
    {
        if (soundOn)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }
    }
}
