using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractType //defaults to pickup
    {
        Pickup,
        ActiveHide,
        PassiveHide
    }
    [SerializeField] protected string itemName;
    public virtual InteractType Interact(PlayerInteract playerInteract)
    {
        playerInteract.AddItem(itemName);
        Destroy(gameObject);
        return InteractType.Pickup; //tells controller what type it was (in this case, makes controller know this item was destroyed
    }

    //used to add to inventory
    public string GetName()
    {
        return itemName;
    }
}
