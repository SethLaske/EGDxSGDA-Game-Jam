using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInteract : MonoBehaviour
{

    //inventory is a dictionary,key = itemname, int = amount of the item
    private Dictionary<string, int> inv = new Dictionary<string, int>();
    //private Collider2D lastInteract;
    private List<Collider2D> lastInteract;

    string[] raycastIgnoreLayers;
    LayerMask raycastMask;

    [SerializeField] private GameObject honey;

    // Start is called before the first frame update
    void Start()
    {
        lastInteract = new List<Collider2D>();

        //governs raycast
        raycastIgnoreLayers = new string[2]; //have to resize :((
        raycastIgnoreLayers[0] = "Ignore Raycast";
        raycastMask = LayerMask.GetMask(raycastIgnoreLayers);
        raycastMask = ~raycastMask;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckShoot())
            {
                Debug.Log("Can shoot to that location");
                Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log(mousepos);
                Instantiate<GameObject>(honey, new Vector3(mousepos.x, mousepos.y, 0), Quaternion.identity);
            }
            else
            {
                Debug.Log("Can't shoot to that location");
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) //interact
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
            }
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
    }

    //
    //end interaction system



    //start honey shooting system
    private bool CheckShoot()
    {
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
    }
}
