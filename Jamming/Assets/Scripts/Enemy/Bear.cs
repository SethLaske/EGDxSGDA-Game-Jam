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
    public LayerMask obstacleLayer;
    public Transform seen;
    public Vector3 lastposition;

    // Start is called before the first frame update
    void Start()
    {
        nextpost = firstpost.transform.position;
        StartCoroutine(ChangeDirection());
    }

    // Update is called once per frame
    void Update()
    {
        if (seen != null)
        {
            //Rotates towards object and speeds up
            Vector3 targetvector = (seen.position - transform.position).normalized;
            float angle = Mathf.Atan2(targetvector.y, targetvector.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);
            Debug.Log("Roatating");
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 2 * rotatespeed * Time.deltaTime);
            rb.velocity = (seen.position - transform.position).normalized * 2 * movespeed;

            //Checks line of sight again
            Vector3 direction = seen.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, obstacleLayer);
            if (hit.collider != null)
            {
                lastposition = seen.position;
                seen = null;
                StartCoroutine(ChangeDirection());
            }
        }
        else if (seen != null) { 
            //NavMesh to seen
            //Once arrived do 360
            //NavMesh to next point
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "path") {
            nextpost = collision.GetComponent<PathPost>().nextpost.transform.position;
            if (seen == null) {
                StartCoroutine(ChangeDirection());
            }
            
        }

        else if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Eaten");
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

    public void ApproachSight(Transform Location)
    {
        seen = Location;
        Debug.Log("The bear has seen something and will go straight to it");
    }

    public void ApproachAudio(Vector3 Location) {
        Debug.Log("The bear is bothered and will approach the noise");
    }
}
