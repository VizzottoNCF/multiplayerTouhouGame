using UnityEngine;
using Unity.Netcode;

public class TopDownMovement : NetworkBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody2D rb2d;
    private Vector2 moveInput;

    private void Update()
    {
        // doesn't run code if character doesn't belong to player
        //if (!IsOwner) { return; }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // normalizes diagonals 
        moveInput.Normalize();

        rb2d.linearVelocity = moveInput * moveSpeed;
    }

}
