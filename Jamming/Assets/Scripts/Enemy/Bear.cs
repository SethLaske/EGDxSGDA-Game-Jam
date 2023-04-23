using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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

    public AudioSource charge;
    public AudioSource eathoney;
    public AudioSource stung;


    private MusicController worldMusic;
    private bool hasAddedToCount;

    public CinemachineVirtualCamera virtualCamera;
    //animation
    private Animator animator;
    //bools: Eating, Running, Attacking

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GameObject.FindGameObjectWithTag("Cinema").GetComponent<CinemachineVirtualCamera>();
        state = "Patrol";
        nextpost = firstpost.transform.position;
        agent.SetDestination(nextpost);
        //StartCoroutine(ChangeDirection());
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        worldMusic = GameObject.Find("Music Controller").GetComponent<MusicController>();
        hasAddedToCount = false;
        animator = GetComponentInChildren<Animator>();
    }

    private void setAnims(int boolToSet)
    {
        if(boolToSet == 0)
        {
            animator.SetBool("isEating", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
        }
        else if(boolToSet == 1)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isEating", false);
            animator.SetBool("isAttacking", false);
        }
        else
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("isEating", false);
            animator.SetBool("isRunning", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.velocity.magnitude > 1) {
            rotate(agent.velocity);
        }



        if (state == "Chase")
        {
            setAnims(1); //run
            Chase();
            if (!hasAddedToCount)
            {
                hasAddedToCount = true;
                worldMusic.AddBear();
            }
        }
        else if (honeybrain.Count > 0 && hunting == false)
        {
            if (honeybrain[0] == null)
            {
                honeybrain.RemoveAt(0);
            }
            else
            {
                state = "GoToHoney";
                Debug.Log("Moving towards honey");
                GoToHoney();
            }
        }
        else if (state == "Approach")
        {
            setAnims(1);
            Approach();
        }
        else if (state == "Patrol")
        {
            setAnims(1);
            Patrol();
        }
        if (state != "Chase")
        {
            if (hasAddedToCount)
            {
                hasAddedToCount = false;
                worldMusic.RemoveBear();
            }
        }

    }

    public void Patrol() {
        //Nothing happens, kinda just follows path
        agent.speed = walkspeed;
        //honeybrain.Clear();
    }

    public void Approach() {
        agent.speed = runspeed;
        if (agent.remainingDistance > .4f)
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
        
        
        PathPost post = collision.GetComponent<PathPost>();
        if (post != null)
        {
            nextpost = post.nextpost.transform.position;
            if (post.pathnumber == pathnumber && hunting == false) {
                //StartCoroutine(ChangeDirection());
                agent.SetDestination(nextpost);
            }
            
        }

        else if (collision.gameObject.tag == "Player")
        {
            setAnims(2); //attack
            firstcollider.enabled = false;
            secondcollider.enabled = false;
            Debug.Log("Player contacted");
            PlayerInteract player = collision.GetComponent<PlayerInteract>();
            int beesused = player.RemoveItem("Bee", 5);
            Debug.Log(beesused + " Bees used to save player");
            Stun(beesused);

            if (beesused == 0)
            {
                Debug.Log("GAMEOVER");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else {
                
                float shakeDuration = 0.5f;
                float shakeAmplitude = 1.2f;
                float shakeFrequency = 2.0f;

                CinemachineImpulseSource impulseSource;


    // Get the Cinemachine Impulse Source component from the virtual camera
                impulseSource = virtualCamera.GetComponent<CinemachineImpulseSource>();



                // Trigger a screen shake effect when the player collides with the object
                //impulseSource.GenerateImpulse(new Vector2(shakeAmplitude, shakeAmplitude), shakeFrequency, shakeDuration);    
                impulseSource.GenerateImpulse();
            }
            //gameover
        }
    }

    

    IEnumerator EatHoney() {
        setAnims(0);
        eathoney.Play();
        firstcollider.enabled = false;
        secondcollider.enabled = false;
        eating = true;
        sight.enabled = false;
        yield return new WaitForSeconds(2.5f);
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
            if (hunting == false) {
                charge.Play();
            }
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
        charge.Stop();
        stung.Play();
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
