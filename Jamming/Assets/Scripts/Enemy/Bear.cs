using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
    public float movespeed;
    public float rotatespeed;
    public Rigidbody2D rb;
    public PathPost firstpost;
    public Vector3 nextpost;
    public Vector3 distractor;

    // Start is called before the first frame update
    void Start()
    {
        nextpost = firstpost.transform.position;
        StartCoroutine(ChangeDirection());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("down * rotation = " + transform.rotation * (Vector3.down));
        if (distractor != null)
        {
            //rb.velocity = (nextpost - transform.position).normalized * movespeed;
        }
        else {
            //rb.velocity = (distractor - transform.position).normalized * movespeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "path") {
            nextpost = collision.GetComponent<PathPost>().nextpost.transform.position;
            StartCoroutine(ChangeDirection());
        }

        else if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player spotted");
            PlayerInteract player = collision.GetComponent<PlayerInteract>();
            StartCoroutine(ChangeDirection());
        }
    }

    IEnumerator ChangeDirection() {
        rb.velocity = Vector2.zero;
        Vector3 targetvector = (nextpost - transform.position).normalized;
        float angle = Mathf.Atan2(targetvector.y, targetvector.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);
        while (transform.rotation != targetRotation)
        {
            Debug.Log("Roatating");
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotatespeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //transform.rotation = Quaternion.Euler(0, 0, angle + 90);

        //yield return new WaitForSeconds(1f);
        rb.velocity = (nextpost - transform.position).normalized * movespeed;
    }

    private float normalize_angle_left_right(float angle)
    {
        while (angle < -Mathf.PI) angle += 2 * Mathf.PI;
        while (angle > Mathf.PI) angle -= 2 * Mathf.PI;
        return angle;
    }

}
