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
            
            float chance = chancemultiplier * sound.sound / Vector3.Distance(sound.transform.position, bear.transform.position);

            if (Random.Range(0, chance) < 1) {
                bear.ApproachAudio(sound.transform.position);
            }
        }
    }
}
