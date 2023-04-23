using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHurtbox : MonoBehaviour
{
    BossScript mainScript;
    private void Start()
    {
        mainScript = GetComponentInParent<BossScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log(collision.name);
        if (collision.name == "HoneyProjectile(Clone)" || collision.name == "HoneyTemp(Clone)")
        {
            Destroy(collision.gameObject);
            mainScript.Damage();
        }
    }
}
