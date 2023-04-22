using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public Bear bear;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player seen");
            PlayerInteract player = collision.GetComponent<PlayerInteract>();

            Vector3 direction = player.transform.position - transform.position;

            // Cast a ray in that direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, bear.obstacleLayer);

            // Check if the ray hit an obstacle
            if (hit.collider != null)
            {
                Debug.Log("Obstacle detected: " + hit.collider.name);
            }
            else {
                bear.ApproachSight(player.transform);
            }
                
        }

        Honey honey = collision.GetComponent<Honey>();
        if (honey != null) {
            bear.ApproachHoney(honey);
        }
    }
}
