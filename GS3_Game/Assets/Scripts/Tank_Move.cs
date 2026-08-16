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
    public float globalSpeedModifier = 1f;
    [SerializeField] private float leftTrack;
    [SerializeField] private float rightTrack;
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
        transform.Rotate(Vector3.up * movement.x * turnSpeed * Time.fixedDeltaTime);
        float forward = movement.y;
        float turn = movement.x;

        leftTrack = forward + turn;
        rightTrack = forward - turn;

        float max = Mathf.Max(Mathf.Abs(leftTrack), Mathf.Abs(rightTrack));

        if (max > 1f)
        {
            leftTrack /= max;
            rightTrack /= max;
        }

        anim.SetFloat("L_Speed", leftTrack * globalSpeedModifier);
        anim.SetFloat("R_Speed", rightTrack * globalSpeedModifier);
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
