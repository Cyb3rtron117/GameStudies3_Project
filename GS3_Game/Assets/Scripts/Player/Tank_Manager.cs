using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor.Build.Content;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Tank_Manager : MonoBehaviour
{
    [Header("Moving")]
    private Vector2 movement;
    public Rigidbody rb;
    public Animator anim;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 10f;
    private bool isMoving = false;
    
    [Header("Shooting")]
    public float shootCooldown = 1f;
    private float cooldownTime;
    
    public bool canShoot = false;
    public float ShakeIntensity = 1f;
    public float ShakeTime = 0.5f;
    [Header("Tank Prefabs")]
    public List<GameObject> TankPrefabs = new List<GameObject>();
    private GameObject currentTank;
    [SerializeField] private TankPrefab tankprefab;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform barrel;
    private List<Material> colours = new List<Material>();
    public int tankIndex = 0;
    public int colourIndex = 0;
    public Material activeMaterial;
    [Header("Game Systems")]
    public GameObject gameManager;
    public Team _team; //set by PlayerSpawning
    [Header("Cinemachine")]
    [SerializeField] private CinemachineOrbitalFollow cineOrbit;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        cineOrbit = GetComponentInChildren<CinemachineOrbitalFollow>();
        colours = gameManager.GetComponent<Colours>().colours;
        ApplyColour();
        ChangeTank(TankPrefabs[tankIndex]);
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
        //transform.Rotate(Vector3.up * movement.x * turnSpeed * Time.fixedDeltaTime);

        //REVERSE INVERSE
        if (movement.y < 0)
        {
            transform.Rotate(Vector3.up * movement.x * -turnSpeed * Time.fixedDeltaTime);
        }
        else
        {
            transform.Rotate(Vector3.up * movement.x * turnSpeed * Time.fixedDeltaTime);
        }
        
    }
    private void Shoot()
    {
        if (canShoot)
        {
            anim.SetTrigger("Shoot");
            canShoot = false;
            cooldownTime = shootCooldown;
            CinemachineShake.Instance.shakeCam(ShakeIntensity, ShakeTime);
            Vector3 shootDir = new Vector3(0f,turret.eulerAngles.y, 0f);
            Projectile.Instance.Shoot(barrel, shootDir, gameObject, _team);
        }
    }
    private void RotateTurret()
    {
        turret.transform.eulerAngles = new Vector3(0, cineOrbit.HorizontalAxis.Value, 0);
    }

    private void FixedUpdate()
    {
        Move();
        RotateTurret();
        //SHOOT COOLDOWN
        /*
        if(cooldownTime > 0)
        {
            cooldownTime -= Time.deltaTime;
            if(cooldownTime <= 0)
            {
                canShoot = true;
            }
        }*/
    }

    public void changeMaterial(Material newMat)
    {
        tankprefab.changeMaterial(newMat);
        
    }
    public void ChangeTank(GameObject tank)
    {
        if (currentTank != null)
        {
            currentTank.SetActive(false);
        }

        currentTank = tank;
        currentTank.SetActive(true);

        tankprefab = currentTank.GetComponent<TankPrefab>();

        turret = tankprefab.turret;
        barrel = tankprefab.barrel;
        anim = tankprefab.animator;
        moveSpeed = tankprefab.moveSpeed;
        turnSpeed = tankprefab.turnSpeed;
        GetComponent<BoxCollider>().size = tankprefab.collider.size;
        GetComponent<BoxCollider>().center = tankprefab.collider.center;
        rb.mass = tankprefab.tankweight;

        changeMaterial(activeMaterial);
    }

    public void ChangeColour(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        float direction = context.ReadValue<float>();

        if (direction > 0.5f)
        {
            NextColour();
        }
        else if (direction < -0.5f)
        {
            PreviousColour();
        }
    }
    private void NextColour()
    {
        colourIndex++;

        if (colourIndex >= colours.Count)
        {
            colourIndex = 0;
        }

        ApplyColour();
    }
    private void PreviousColour()
    {
        colourIndex--;

        if (colourIndex < 0)
        {
            colourIndex = colours.Count - 1;
        }

        ApplyColour();
    }
    private void ApplyColour()
    {
        activeMaterial = colours[colourIndex];

        if (tankprefab != null)
        {
            tankprefab.changeMaterial(activeMaterial);
        }
    }

}
