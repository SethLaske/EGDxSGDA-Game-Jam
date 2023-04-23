using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    [SerializeField] int health = 5;
    [SerializeField] float rotateSpeed = 60f;
    int dir = 1;
    public void Damage()
    {
        health -= 1;
        dir *= -1;
    }

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime * dir));
    }
}
