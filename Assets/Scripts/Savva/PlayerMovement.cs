using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement")]
    public float jumpVelocity;
    public float horizontalVelocity;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    InputAction moveAction;
    InputAction jumpAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Updates every frame.
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        // Update horizontal velocity
        rb.linearVelocityX = moveValue.x * horizontalVelocity;

        // Jump if on ground
        if (jumpAction.IsPressed() && onGround())
        {
            rb.linearVelocityY = jumpVelocity;
        }
    }

    private bool onGround()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
    }

    // Draws Gizmos around ground collider for jumps.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);
    }
}
