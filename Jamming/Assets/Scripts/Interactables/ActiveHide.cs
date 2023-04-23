using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveHide : Interactable
{
    private HideIndicator ind;
    private bool h;
    private void Start()
    {
        ind = GetComponentInChildren<HideIndicator>();
        h = false;
    }
    public override InteractType Interact(PlayerInteract playerInteract)
    {
        h = !h;
        ind.PlayerHide(h);
        return InteractType.ActiveHide; //tells controller what type it was (in this case, makes controller know this item was destroyed
    }
}
