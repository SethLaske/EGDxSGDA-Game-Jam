using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearing : MonoBehaviour
{
    public Bear bear;
    public float chancemultiplier;
    private void OnTriggerStay2D(Collider2D collision)
    {
        SoundObject sound = collision.GetComponent<SoundObject>();
        if (sound != null)
        {
            Debug.Log("Something heard");
            if (sound.sound == 0) {
                return;
            }
            float chance = chancemultiplier * Vector3.Distance(sound.transform.position, bear.transform.position)/ sound.sound;

            if (Random.Range(0, chance) < 1) {
                bear.ApproachAudio(sound.transform.position);
            }
        }
    }
}
