using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIndicator : MonoBehaviour
{
    private Transform sprite;
    [SerializeField] private SpriteRenderer spriteRend;

    private void Start()
    {
        sprite = transform.GetChild(0);
    }

    public void PlayerHide(bool playerInside)
    {
        if (playerInside)
        {
            Debug.Log("Inside");
            spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 1f);
        }
        else
        {
            Debug.Log("Outside");
            spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, .5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            sprite.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            sprite.gameObject.SetActive(false);
        }
    }
}
