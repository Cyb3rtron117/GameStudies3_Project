using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Tank_Move : MonoBehaviour
{
    private Vector2 movement;
    public Rigidbody rb;
    public Animator anim;
    public float moveSpeed = 10f;
    public float turnSpeed = 10f;
    private bool isMoving = false;
    [Header("Shooting")]
    public float shootCooldown = 1f;
    private float cooldownTime;
    [SerializeField] private bool canShoot = true;
    public float ShakeIntensity = 1f;
    public float ShakeTime = 0.5f;

    private void Start()
    {
        cooldownTime = shootCooldown;
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        if(movement.sqrMagnitude>0)
        {
            isMoving = true;
            anim.SetBool("isMoving", isMoving);
        }
        else
        {
            isMoving = false;
            anim.SetBool("isMoving", isMoving);
        }
        //Debug.Log(movement);
    }
    public void ShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shoot();
        }
    }

    private void Move()
    {
        rb.linearVelocity = transform.forward * movement.y * moveSpeed;
        //REGULAR   
        transform.Rotate(Vector3.up * movement.x * turnSpeed * Time.fixedDeltaTime);

        //REVERSE INVERSE
        /*if (movement.y < 0)
        {
            transform.Rotate(Vector3.up * movement.x * -turnSpeed * Time.fixedDeltaTime);
        }
        else
        {
            transform.Rotate(Vector3.up * movement.x * turnSpeed * Time.fixedDeltaTime);
        }*/
        
    }
    private void Shoot()
    {
        if (canShoot)
        {
            anim.SetTrigger("Shoot");
            canShoot = false;
            cooldownTime = shootCooldown;
            CinemachineShake.Instance.shakeCam(ShakeIntensity, ShakeTime);
        }
    }

    private void Update()
    {
        Move();
        if(cooldownTime > 0)
        {
            cooldownTime -= Time.deltaTime;
            if(cooldownTime <= 0)
            {
                canShoot = true;
            }
        }
    }
}
