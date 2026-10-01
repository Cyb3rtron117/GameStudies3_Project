using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Tank_Manager : MonoBehaviour
{
    [SerializeField] private float Health = 100f;
    public float maxHealth = 100f;
    public bool freeze = false;
    public int playerIndex;
    [Header("Moving")]
    private Vector2 movement;
    public Rigidbody rb;
    public Animator anim;
    private bool isMoving = false;
    [SerializeField] private float boostTime = 0.5f;
    [SerializeField] private float boostCooldown = 1f;
    [SerializeField] private bool isBoosting = false;

    [Header("Speeds")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float currentSpeed = 10f;
    [SerializeField] private float turnSpeed = 10f;
    
    
    [Header("Shooting")]
    public float shootCooldown = 1f;
    private float cooldownTime;
    
    public bool canShoot = false;
    public float ShakeIntensity = 1f;
    public float ShakeTime = 0.5f;
    public GameObject canvasObjects;

    [Header("Tank Prefabs")]
    public List<GameObject> TankPrefabs = new List<GameObject>();
    private GameObject currentTank;
    [SerializeField] private TankPrefab tankprefab;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform barrel;
    private List<Material> colours = new List<Material>();
    public int tankIndex = 0;
    //public int colourIndex = 0;
    public Material activeMaterial;
    public VisualEffect shootVFX;
    public VisualEffect impactVFX;
    public VisualEffect healthVFX;
    public float ShootDamage = 0f;
    private float projectileSpeed;

    [Header("Game Systems")]
    public GameObject gameManager;
    public Team _team; //set by PlayerSpawning
    public TextMeshProUGUI teamNumber;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineOrbitalFollow cineOrbit;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        cineOrbit = GetComponentInChildren<CinemachineOrbitalFollow>();
        colours = gameManager.GetComponent<Colours>().colours;
        ApplyColour();
        ChangeTank(TankPrefabs[tankIndex]);
        canvasObjects.SetActive(false);
        isBoosting = false;
        teamNumber.SetText($"You are {_team}");
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
    public void BoostInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isBoosting && !freeze)
        {
            isBoosting = true;
            StartCoroutine(Boost(boostTime));
        }
    }
    private void Move()
    {
        if(!freeze)
        {
            rb.linearVelocity = transform.forward * movement.y * currentSpeed;
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
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    public void HasProjectile()
    {
        canShoot = true;
        canvasObjects.SetActive(true);
    }
    private void Shoot()
    {
        if (canShoot && !freeze)
        {
            anim.SetTrigger("Shoot");
            canShoot = false;
            cooldownTime = shootCooldown;
            CinemachineShake.Instance.shakeCam(ShakeIntensity, ShakeTime);
            Vector3 shootDir = new Vector3(0f,turret.eulerAngles.y, 0f);
            Projectile.Instance.Shoot(barrel, shootDir, gameObject, _team, ShootDamage, projectileSpeed);
            canvasObjects.SetActive(false);
            shootVFX.Play();
        }
    }
    IEnumerator Boost(float waitTime)
    {
        currentSpeed = boostSpeed;
        yield return new WaitForSeconds(waitTime);
        currentSpeed = moveSpeed;
        StartCoroutine(BoostCooldown(boostCooldown));
    }
    IEnumerator BoostCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isBoosting = false;
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
        float _health = Health / maxHealth;
        healthVFX.SetFloat("FillAmt", _health);
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
        boostSpeed = moveSpeed + 5;
        currentSpeed = moveSpeed;
        turnSpeed = tankprefab.turnSpeed;
        GetComponent<BoxCollider>().size = tankprefab.collider.size;
        GetComponent<BoxCollider>().center = tankprefab.collider.center;
        rb.mass = tankprefab.tankweight;
        shootVFX = tankprefab.shootVFX;
        impactVFX = tankprefab.impactVFX;
        healthVFX = tankprefab.healthbar;
        changeMaterial(activeMaterial);
        ShootDamage = tankprefab.DamageAmt;

        maxHealth = tankprefab.maxHealth;
        Health = maxHealth;

        projectileSpeed = tankprefab.shootSpeed;
    }
    private void ApplyColour()
    {
        if (tankprefab != null)
        {
            tankprefab.changeMaterial(activeMaterial);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Tank"))
        {
            if (isBoosting)
            {
                Tank_Manager tankScript = collision.gameObject.GetComponent<Tank_Manager>();
                if (tankScript._team != _team) //if not on my team
                {
                    if (tankScript.canShoot == true) //if they have the bullet
                    {
                        CinemachineShake.Instance.shakeCam(ShakeIntensity, ShakeTime);
                        tankScript.canShoot = false;
                        tankScript.canvasObjects.SetActive(false);
                        Projectile.Instance.Bounce(collision.transform);
                    }
                    tankScript.TakeDamage(5);
                }
                impactVFX.Play();
            }
        }
    }
    public void TakeDamage(float amount)
    {
        print("damaged");
        Health -= amount;
        if(Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (canShoot) //if tank has the bullet
        {
            CinemachineShake.Instance.shakeCam(ShakeIntensity, ShakeTime);
            canShoot = false;
            canvasObjects.SetActive(false);
            Projectile.Instance.Bounce(transform);
        }
        freeze = true;
        if (currentTank != null)
        {
            currentTank.SetActive(false);
        }
        transform.position = gameManager.GetComponent<PlayerSpawning>().SpawnPoints[playerIndex].position;
        transform.eulerAngles = gameManager.GetComponent<PlayerSpawning>().SpawnPoints[playerIndex].eulerAngles;
        StartCoroutine(Respawn(2));
    }
    IEnumerator Respawn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        freeze = false;
        Health = maxHealth;
        if (currentTank != null)
        {
            currentTank.SetActive(true);
        }
    }
}
