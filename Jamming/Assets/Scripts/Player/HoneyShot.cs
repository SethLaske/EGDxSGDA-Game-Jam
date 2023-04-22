using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HoneyShot : MonoBehaviour
{
    private float speed;
    private Vector2 target;
    [SerializeField] private GameObject honey;
    // Start is called before the first frame update
    void Start()
    { 
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if(Vector2.Distance(transform.position, target) < 0.2f) //magic number, can adjust as needed
        {
            MakeHoney();
        }
    }

    public void SetVars(Vector2 _target, float _speed = 7f)
    {
        this.speed = _speed;
        this.target = _target;
    }

    private void MakeHoney()
    {
        honey = Instantiate<GameObject>(honey, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Player" && collision.tag != "Interactable" && collision.tag != "NoStop")
        {
            MakeHoney();
        }
    }
}
