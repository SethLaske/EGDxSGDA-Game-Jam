using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;


public class PlayerInteract : MonoBehaviour
{

    //movement
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public Vector2 forceToApplyOnPlayer;
    Vector2 playerInput;
    public float forceOnSide = 5f;
    


    //public float sound;
    //inventory is a dictionary,key = itemname, int = amount of the item
    private Dictionary<string, int> inv = new Dictionary<string, int>();
    //private Collider2D lastInteract;
    private List<Collider2D> lastInteract;

    //honey
    string[] raycastIgnoreLayers;
    LayerMask raycastMask;
    [SerializeField] private GameObject honey;
    public SoundObject sound;

    //stealth
    public bool hidden;
    private bool stealthTerrain;
    private SpriteRenderer sprite;


    //animation
    //public bool moving;
    //public bool shooting;
    private Animator animator;


    // Start is called before the first frame update
    public TextMeshProUGUI honeytext;
    public TextMeshProUGUI beetext;

    //rotation lock
    bool lockRot;

    //firepoint
    [SerializeField] Transform firePoint;

    void Start()
    {
        //interact
        lastInteract = new List<Collider2D>();

        //governs raycast
        raycastIgnoreLayers = new string[2]; //have to resize :((
        raycastIgnoreLayers[0] = "Ignore Raycast";
        raycastMask = LayerMask.GetMask(raycastIgnoreLayers);
        raycastMask = ~raycastMask;
        inv["honey"] = 5;
        inv["Bee"] = 0;

        //stealth
        hidden = false;
        stealthTerrain = false;
        sprite = GetComponent<SpriteRenderer>();

        UpdateUI();

        animator = GetComponentInChildren<Animator>();
        lockRot = false;
    }


    // Update is called once per frame
    void Update()
    {

        //player input
        playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        //interact
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckShoot())
            {
                
                animator.SetBool("Shooting", true);
                Debug.Log("ammo: " + inv["honey"]);
                Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Debug.Log(mousepos);
                GameObject honeyshot = Instantiate<GameObject>(honey, firePoint.position, Quaternion.identity);
                //Instantiate<GameObject>(honey, new Vector3(mousepos.x, mousepos.y, 0), Quaternion.identity);
                honeyshot.transform.up = new Vector3(mousepos.x, mousepos.y, honeyshot.transform.position.z) - honeyshot.transform.position;
                honeyshot.GetComponent<HoneyShot>().SetVars(mousepos);


                if (mousepos.x < transform.position.x) //
                {
                    transform.rotation = new Quaternion(0, -180, 0, 0);
                    firePoint.position = new Vector3(transform.position.x + 0.4440002f, firePoint.position.y, 0);
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                    firePoint.position = new Vector3(transform.position.x - 0.4440002f, firePoint.position.y, 0);
                }
                StartCoroutine(LockRotation(0.45f)); //lock rotation for firing

                inv["honey"] -= 1;
                UpdateUI();
            }
            else
            {
                Debug.Log("Can't shoot");
            }
        }
        else
        {
            animator.SetBool("Shooting", false);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E)) //interact
        {
            int listSize = lastInteract.Count;
            if (listSize != 0) // != null 
            {
                Collider2D obj = lastInteract[listSize - 1];
                Interactable.InteractType type = obj.GetComponent<Interactable>().Interact(this);
                if (type == Interactable.InteractType.Pickup)
                {
                    lastInteract.Remove(obj); //technically this *should* be triggered with the ontriggerexit, but this is for safety.
                }
                else if (type == Interactable.InteractType.ActiveHide)
                {
                    hidden = !hidden;
                    if (hidden)
                    {
                        sound.sound = 0;
                        transform.position = obj.transform.position;
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
                    }
                    else
                    {
                        sound.sound = inv["Bee"] * 3 + 5;
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 255);
                    }
                }
            }
        }
    }

    //movement
    void FixedUpdate()
    {
        if (!hidden)
        {
            float input = Input.GetAxis("Horizontal");
            if (!lockRot)
            {
                if (input > 0)
                {
                    transform.rotation = Quaternion.identity;
                }
                else if (input < 0)
                {
                    transform.rotation = new Quaternion(0, -180, 0, 0);
                }
            }

            // Movement
            Vector2 moveForce = playerInput * moveSpeed;
            moveForce = moveForce + forceToApplyOnPlayer;
            forceToApplyOnPlayer /= forceOnSide;
            if (Mathf.Abs(forceToApplyOnPlayer.x) <= 0.01f && Mathf.Abs(forceToApplyOnPlayer.y) <= 0.01f)
            {
                animator.SetBool("Moving", true);
                forceToApplyOnPlayer = Vector2.zero;
            }
            else
            {
                animator.SetBool("Moving", true);
            }
            rb.velocity = moveForce;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }


    //start interaction system
    //

    //add X amount of an item
    public void AddItem(string item, int amount = 1)
    {
        try
        {
            inv[item] += amount;
        }
        catch
        {
            inv[item] = amount;
        }
        Debug.Log(item + " amount: " + inv[item]);
        if (item == "Bee") {
            sound.sound = inv[item] * 3 + 5;
        }
        UpdateUI();
        
    }

    //returns the actual amount of the item removed (if it returns 0, that means there was no item
    public int RemoveItem(string item, int amount = 1)
    {
        try
        {
            inv[item] -= amount; 
            if (inv[item] <= 0){ //calculate the difference between amount to remove and amount actually removed
                int diff = inv[item] * -1;
                inv[item] = 0;
                UpdateUI();
                return amount - diff;   
            }
            UpdateUI();
            return amount;
            
        }
        catch
        {
            return 0;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Interactable")
        {
            lastInteract.Add(collision); //know we can interact
        }
        else if(collision.tag == "StealthTerrain")
        {
            Debug.Log("Entered Grass");
            stealthTerrain = true;
        }

    }

    private bool isInInteractList(Collider2D collision)
    { 
        foreach(Collider2D col in lastInteract)
        {
            if(col == collision)
            {
                return true;
            }
        }
        return false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Interactable" && isInInteractList(collision))
        {
            lastInteract.Remove(collision);
        }
        else if (collision.tag == "StealthTerrain")
        {
            Debug.Log("Exited Grass");
            stealthTerrain = false;
        }

    }

    //
    //end interaction system

    public void UpdateUI() {
        if (beetext != null)
        {
            beetext.text = "" + inv["Bee"];
            honeytext.text = "" + inv["honey"];
        }
    }

    //start honey shooting system
    private bool CheckShoot()
    {

        //will eventually have ammo.

        //mouse beyond wall detection - not needed.
        /*
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(transform.position);
        Debug.DrawRay(transform.position, mousepos - transform.position);

        float dist = Vector2.Distance(transform.position, mousepos);



        RaycastHit2D hit = Physics2D.Raycast(transform.position, mousepos - transform.position, dist, raycastMask); //invert mask
        try
        {
            Debug.Log(hit.collider.gameObject.name);
            return false;
        }
        catch //hit nothing
        {
            return true;
        }  
        */
        if(inv["honey"] > 0 && hidden == false){
            return true;
        }
        return false;
        
    }

    IEnumerator LockRotation(float time)
    {
        lockRot = true;
        yield return new WaitForSeconds(time);
        lockRot = false;
    }
}
