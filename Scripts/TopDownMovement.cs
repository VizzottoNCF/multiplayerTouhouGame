using UnityEngine;
using Unity.Netcode;

public class TopDownMovement : NetworkBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody2D rb2d;
    private Vector2 moveInput;
    private Animator _animator;

    private void Awake() { _animator = GetComponent<Animator>(); }
    private void Update()
    {
        //Debug.Log("My owner is " + OwnerClientId + " (top down movement)");
        // doesn't run code if character doesn't belong to player
        if (!IsOwner) { return; }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // normalizes diagonals 
        moveInput.Normalize();

        _animator.SetBool("Shooting", !Input.GetKey(KeyCode.Space));

        rb2d.linearVelocity = moveInput * moveSpeed;
    }
}
