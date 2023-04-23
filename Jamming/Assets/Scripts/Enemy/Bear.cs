using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bear : MonoBehaviour
{
    public string state;

    public int pathnumber;

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
    //public Honey seenhoney;
    public List<Honey> honeybrain = new List<Honey>();
    //public Vector3 direction;
    public bool hunting;
    public bool eating;

    public float degreestorotate = 0;

    public CircleCollider2D firstcollider;
    public CircleCollider2D secondcollider;

    public Collider2D sight;

    // Start is called before the first frame update
    void Start()
    {
        state = "Patrol";
        nextpost = firstpost.transform.position;
        agent.SetDestination(nextpost);
        //StartCoroutine(ChangeDirection());
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.velocity.magnitude > 1) {
            rotate(agent.velocity);
        }

        

        if (state == "Chase")
        {
            Chase();
        }
        else if (honeybrain.Count > 0 && hunting == false)
        {
            if (honeybrain[0] == null)
            {
                honeybrain.RemoveAt(0);
            }
            else {
                state = "GoToHoney";
                Debug.Log("Moving towards honey");
                GoToHoney();
            }
        }
        else if (state == "Approach")
        {
            Approach();
        }
        else if (state == "Patrol") {
            Patrol();
        }
        

    }

    public void Patrol() {
        //Nothing happens, kinda just follows path
        agent.speed = walkspeed;
        //honeybrain.Clear();
    }

    public void Approach() {
        agent.speed = runspeed;
        if (agent.remainingDistance > .6f)
        {
            Debug.Log("Approaching a location");
            //Debug.Log("Remaing distance: " + agent.remainingDistance);
            return;
        }
        //agent.ResetPath();

        //Do a full 360
        
        if (degreestorotate < 360)
        {
            Debug.Log("doing a 360");
            degreestorotate += rotatespeed * Time.deltaTime;
            transform.Rotate(Vector3.forward, rotatespeed * 1 * Time.deltaTime);
            return;
        }
        degreestorotate = 0;


        //go to the next thing

        hunting = false;
        agent.speed = walkspeed;
        agent.SetDestination(nextpost);
        state = "Patrol";
    }

    public void Chase() {
        agent.SetDestination(seen.position);
        Vector3 direction = seen.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, obstacleLayer);
        if (hit.collider != null)
        {
            //lastposition = seen.position;

            //agent.SetDestination(lastposition);
            
            seen = null;
            agent.speed = walkspeed;
            state = "Approach";
            degreestorotate = 0;
            honeybrain.Clear();
        }
    }

    public void GoToHoney()
    {
        agent.SetDestination(honeybrain[0].transform.position);
        if (agent.remainingDistance > .6f)
        {
            Debug.Log("Approaching last honey");
            //Debug.Log("Remaing distance: " + agent.remainingDistance);
            return;
        }
        Debug.Log("Arrived at honey");
        if (eating == false) {
            state = "EatingHoney";
            Debug.Log("Eating honey");
            StartCoroutine(EatHoney());
        }
        
    }

        private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "path") {
            PathPost post = collision.GetComponent<PathPost>();
            nextpost = post.nextpost.transform.position;
            if (post.pathnumber == pathnumber && hunting == false) {
                //StartCoroutine(ChangeDirection());
                agent.SetDestination(nextpost);
            }
            
        }

        else if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player contacted");
            PlayerInteract player = collision.GetComponent<PlayerInteract>();
            int beesused = player.RemoveItem("Bee", 5);
            Debug.Log(beesused + " Bees used to save player");
            Stun(beesused);
            if (beesused == 0) {
                Debug.Log("GAMEOVER");
            }
            //gameover
        }
    }

    

    IEnumerator EatHoney() {
        firstcollider.enabled = false;
        secondcollider.enabled = false;
        eating = true;
        sight.enabled = false;
        yield return new WaitForSeconds(2f);
        //Destroy(seenhoney.gameObject);
        Honey temp = honeybrain[0];

        
        honeybrain.RemoveAt(0);
        Destroy(temp.gameObject);

        state = "Patrol";
        agent.SetDestination(nextpost);
        eating = false;
        
        sight.enabled = true;
        
        firstcollider.enabled = true;
        secondcollider.enabled = true;
        
        //state = "Patrol";
    }

    public void ApproachSight(Transform location)
    {
        if (eating == false) {
            hunting = true;
            seen = location;
            agent.speed = runspeed;
            state = "Chase";
            degreestorotate = 0;
            Debug.Log("The bear has seen something and will go straight to it");
        }
        
    }

    public void ApproachAudio(Vector3 location) {
        if (state == "Patrol") {
            agent.SetDestination(location);
            state = "Approach";
            degreestorotate = 0;
            Debug.Log("The bear is bothered and will approach the noise");
        }
    }

    public void ApproachHoney(Honey honey) {
        
        honeybrain.Add(honey);
            //seenhoney = honey;
            //agent.SetDestination(honey.transform.position);
            //state = "GoToHoney";
        
    }

    public void Stun(float time) {
        if (time > 0) {
            StartCoroutine(StunRoutine(time));
        }
        
    }

    IEnumerator StunRoutine(float time)
    {
        firstcollider.enabled = false;
        secondcollider.enabled = false;

        agent.isStopped = true;
        yield return new WaitForSeconds(time);
        
        agent.isStopped = false;
        agent.SetDestination(transform.position);
        state = "Approach";
        degreestorotate = 0;
        hunting = false;

        firstcollider.enabled = true;
        secondcollider.enabled = true;
    }

    public void rotate(Vector3 targetdirection) {
        targetdirection = targetdirection.normalized;
        float angle = Mathf.Atan2(targetdirection.y, targetdirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotatespeed * Time.deltaTime);
    }
}
