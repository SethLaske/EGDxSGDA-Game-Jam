using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed;
    public Vector2 forceToApplyOnPlayer;
    Vector2 playerInput;
    public float forceOnSide;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Input
        playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    void FixedUpdate ()
    {
        // Movement
        Vector2 moveForce = playerInput * moveSpeed;
        moveForce = moveForce + forceToApplyOnPlayer;
        forceToApplyOnPlayer /= forceOnSide;
        if (Mathf.Abs(forceToApplyOnPlayer.x) <= 0.01f && Mathf.Abs(forceToApplyOnPlayer.y) <=0.01f)
        {
            forceToApplyOnPlayer = Vector2.zero;
        }
        rb.velocity = moveForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }
}
