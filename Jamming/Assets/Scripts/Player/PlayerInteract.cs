using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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


    // Start is called before the first frame update

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
                Debug.Log("ammo: " + inv["honey"]);
                Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Debug.Log(mousepos);
                GameObject honeyshot = Instantiate<GameObject>(honey, transform.position, Quaternion.identity);
                //Instantiate<GameObject>(honey, new Vector3(mousepos.x, mousepos.y, 0), Quaternion.identity);
                honeyshot.transform.up = new Vector3(mousepos.x, mousepos.y, honeyshot.transform.position.z) - honeyshot.transform.position;
                honeyshot.GetComponent<HoneyShot>().SetVars(mousepos);
                inv["honey"] -= 1;
            }
            else
            {
                Debug.Log("Can't shoot");
            }
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
            if (input > 0)
            {
                transform.rotation = Quaternion.identity;
            }
            else if (input < 0)
            {
                transform.rotation = new Quaternion(0, -180, 0, 0);
            }
            // Movement
            Vector2 moveForce = playerInput * moveSpeed;
            moveForce = moveForce + forceToApplyOnPlayer;
            forceToApplyOnPlayer /= forceOnSide;
            if (Mathf.Abs(forceToApplyOnPlayer.x) <= 0.01f && Mathf.Abs(forceToApplyOnPlayer.y) <= 0.01f)
            {
                forceToApplyOnPlayer = Vector2.zero;
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
                return amount - diff;   
            }
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
        if(inv["honey"] > 0){
            return true;
        }
        return false;
        
    }

   
}
