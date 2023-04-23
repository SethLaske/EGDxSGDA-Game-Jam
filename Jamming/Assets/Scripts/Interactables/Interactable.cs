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
    public AudioClip audiofile;
    public virtual InteractType Interact(PlayerInteract playerInteract)
    {
        
        if (audiofile != null) {
            AudioSource soundEffect = (new GameObject("Sound")).AddComponent<AudioSource>();
            soundEffect.clip = audiofile;
            soundEffect.Play();

            // Destroy the game object after the sound effect has finished playing
            Destroy(soundEffect.gameObject, audiofile.length);
        }
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
