using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    //inventory is a dictionary,key = itemname, int = amount of the item
    private Dictionary<string, int> inv = new Dictionary<string, int>();
    //private Collider2D lastInteract;
    private List<Collider2D> lastInteract;

    // Start is called before the first frame update
    void Start()
    {
        lastInteract = new List<Collider2D>();

    }

    // Update is called once per frame
    void Update()
    {
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
}
