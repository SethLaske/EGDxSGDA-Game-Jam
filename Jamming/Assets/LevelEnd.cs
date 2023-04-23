using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelEnd : MonoBehaviour
{
    [SerializeField] string nextScene = "Map";
    public int minimumbees;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PlayerInteract player = collision.GetComponent<PlayerInteract>();
            if (player.RemoveItem("Bee", minimumbees) == minimumbees) {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}
