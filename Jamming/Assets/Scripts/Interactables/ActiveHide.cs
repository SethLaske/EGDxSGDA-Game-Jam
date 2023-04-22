using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveHide : Interactable
{
    public override InteractType Interact(PlayerInteract playerInteract)
    {
        return InteractType.ActiveHide; //tells controller what type it was (in this case, makes controller know this item was destroyed
    }
}
