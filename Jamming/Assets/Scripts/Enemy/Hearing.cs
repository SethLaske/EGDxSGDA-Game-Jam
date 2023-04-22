using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearing : MonoBehaviour
{
    public Bear bear;
    public float chancemultiplier;
    private void OnTriggerStay2D(Collider2D collision)
    { 
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player heard");
            PlayerInteract player = collision.GetComponent<PlayerInteract>();
            float chance = chancemultiplier * player.sound / Vector3.Distance(player.transform.position, bear.transform.position);

            if (Random.Range(0, chance) < 1) {
                bear.ApproachAudio(player.transform.position);
            }
        }
    }
}
