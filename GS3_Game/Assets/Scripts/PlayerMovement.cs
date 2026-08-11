using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 movement;
    public Rigidbody rb;
    public float moveSpeed = 10f;
    public float jumpForce = 5f;
    public void MoveInput(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
    public void JumpInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Jump();
        }
    }
    public void HealInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Healing");
        }
    }

    private void Move()
    {
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.y) * moveSpeed;
    }
    private void Jump()
    {
        rb.AddForce(jumpForce * transform.up, ForceMode.Impulse);
    }

    private void Update()
    {
        Move();
    }
}
