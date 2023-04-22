using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bear : MonoBehaviour
{
    public float walkspeed;
    public float runspeed;
    public float rotatespeed;
    public Rigidbody2D rb;
    public PathPost firstpost;
    public Vector3 nextpost;
    public LayerMask obstacleLayer;
    public Transform seen;
    public Vector3 lastposition;
    public NavMeshAgent agent;
    //public Vector3 direction;
    public bool focused;
    public float degreestorotate = 0;

    // Start is called before the first frame update
    void Start()
    {
        nextpost = firstpost.transform.position;
        agent.SetDestination(nextpost);
        //StartCoroutine(ChangeDirection());
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (degreestorotate == 0) {
            rotate(agent.velocity);
        }
        

        //Debug.Log("Remaining Distance: " + agent.remainingDistance);
        if (seen != null)
        {
            agent.SetDestination(seen.position);
            Vector3 direction = seen.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, obstacleLayer);
            if (hit.collider != null)
            {
                lastposition = seen.position;

                agent.SetDestination(lastposition);
                seen = null;
                StartCoroutine(LostSight());
            }
        }
        else {
            //rotate(agent.velocity);
        }

        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "path") {
            nextpost = collision.GetComponent<PathPost>().nextpost.transform.position;
            if (focused == false) {
                //StartCoroutine(ChangeDirection());
                agent.SetDestination(nextpost);
            }
            
        }

        else if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Eaten");
            //gameover
        }
    }

    /*IEnumerator ChangeDirection() {
        //rb.velocity = Vector2.zero;
        //rotate(nextpost - transform.position);
        Vector3 targetvector = (nextpost - transform.position).normalized;
        float angle = Mathf.Atan2(targetvector.y, targetvector.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);
        while (transform.rotation != targetRotation)
        {
            //Debug.Log("Rotating");
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotatespeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //transform.rotation = Quaternion.Euler(0, 0, angle + 90);

        //yield return new WaitForSeconds(1f);
        //rb.velocity = (nextpost - transform.position).normalized * movespeed;
    }*/

    IEnumerator LostSight() {
        //Approach the last remaining location
        while (agent.remainingDistance > .2f) {
            Debug.Log("Approaching last known location");
            yield return new WaitForSeconds(Time.deltaTime);
        }
        agent.ResetPath();
        //Do a full 360
        Quaternion initialdirection = transform.rotation;

        
        
        while (degreestorotate < 360)
        {
            Debug.Log("doing a 360");
            degreestorotate += rotatespeed * Time.deltaTime;
            transform.Rotate(Vector3.forward, rotatespeed * 1 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        degreestorotate = 0;
        //go to the next thing
        Debug.Log("moving on with my life");
        focused = false;
        agent.speed = walkspeed;
        agent.SetDestination(nextpost);

    }

    private float normalize_angle_left_right(float angle)
    {
        while (angle < -Mathf.PI) angle += 2 * Mathf.PI;
        while (angle > Mathf.PI) angle -= 2 * Mathf.PI;
        return angle;
    }

    public void ApproachSight(Transform Location)
    {
        focused = true;
        seen = Location;
        agent.speed = runspeed;
        Debug.Log("The bear has seen something and will go straight to it");
    }

    public void ApproachAudio(Vector3 Location) {
        Debug.Log("The bear is bothered and will approach the noise");
    }

    public void rotate(Vector3 targetdirection) {
        targetdirection = targetdirection.normalized;
        float angle = Mathf.Atan2(targetdirection.y, targetdirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotatespeed * Time.deltaTime);
    }
}
