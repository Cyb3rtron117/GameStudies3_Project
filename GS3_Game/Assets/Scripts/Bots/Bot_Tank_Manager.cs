using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.Windows;

public class Bot_Tank_Manager : MonoBehaviour
{
    [SerializeField] private float Health = 100f;
    public float maxHealth = 100f;
    public bool freeze = false;
    public int playerIndex;
    public NavMeshAgent agent;
    public GameObject goalTarget;
    [Header("Moving")]
    private float additionalAngle = 0f;
    public Rigidbody rb;
    public Animator anim;
    private bool isMoving = false;
    [SerializeField] private float boostTime = 0.5f;
    [SerializeField] private float boostCooldown = 1f;
    [SerializeField] private bool isBoosting = false;
    public int MoveMultiplier = 1;

    [Header("Speeds")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float currentSpeed = 10f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float aimSpeed = 1f;

    [Header("Shooting")]
    public float shootCooldown = 1f;
    private float cooldownTime;
    
    public bool canShoot = false;
    public float ShakeIntensity = 1f;
    public float ShakeTime = 0.5f;
    private Vector3 aimpos;

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
    public List<GameObject> MyTeam = new List<GameObject>();

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        gameManager = GameObject.FindGameObjectWithTag("GameController");
        colours = gameManager.GetComponent<Colours>().colours;
        ChangeTank(tankIndex);
        isBoosting = false;
    }
    private void Move()
    {
        if(!freeze)
        {
            if (Projectile.priorityObject != gameObject)
            {
                agent.SetDestination(Projectile.priorityObject.transform.position);
            }
            else
            {
                agent.SetDestination(goalTarget.transform.position);
            }
            Vector3 desiredVelocity = agent.desiredVelocity;
            Vector3 targetVelocity = new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z);
            rb.linearVelocity = targetVelocity;

            if (desiredVelocity.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed);
            }

            //Boosting Logic
            Vector3 targetDistance = Projectile.priorityObject.transform.position - transform.position;
            if (Projectile.priorityObject != gameObject && !MyTeam.Contains(Projectile.priorityObject)) //redundant to check for self, because self is already in my team
            {
                Vector3 direction = (Projectile.priorityObject.transform.position - transform.position).normalized;

                if (Vector3.Dot(transform.forward, direction) > 0.8f) //if target is roughly in front of me
                {
                    if (targetDistance.sqrMagnitude < 7f && !isBoosting && !freeze)
                    {
                        isBoosting = true;
                        StartCoroutine(Boost(boostTime));
                    }
                }
            }
            agent.nextPosition = transform.position;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    public void HasProjectile()
    {
        canShoot = true;
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
            shootVFX.Play();
        }
    }
    IEnumerator Boost(float waitTime)
    {
        currentSpeed = boostSpeed;
        agent.nextPosition = transform.position;
        yield return new WaitForSeconds(waitTime);
        currentSpeed = moveSpeed;
        StartCoroutine(BoostCooldown(boostCooldown));
        agent.nextPosition = transform.position;
    }
    IEnumerator BoostCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isBoosting = false;
    }
    private void RotateTurret()
    {
        if (Projectile.priorityObject != gameObject)
        {
            aimpos = Projectile.priorityObject.transform.position - transform.position;
        }
        else
        {
            aimpos = goalTarget.transform.position - transform.position;
        }
        aimpos.y = 0;
        turret.transform.eulerAngles = Quaternion.LookRotation(aimpos).eulerAngles;
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
    public void ChangeTank(int _tankIndex)
    {
        if (currentTank != null)
        {
            currentTank.SetActive(false);
        }
        
        currentTank = TankPrefabs[_tankIndex];
        currentTank.SetActive(true);

        tankprefab = currentTank.GetComponent<TankPrefab>();
        
        turret = tankprefab.turret;
        barrel = tankprefab.barrel;
        anim = tankprefab.animator;
        moveSpeed = tankprefab.moveSpeed;
        boostSpeed = moveSpeed + 5;
        currentSpeed = moveSpeed;
        agent.speed = currentSpeed;
        turnSpeed = tankprefab.turnSpeed;
        GetComponent<BoxCollider>().size = tankprefab.collider.size;
        GetComponent<BoxCollider>().center = tankprefab.collider.center;
        rb.mass = tankprefab.tankweight;
        shootVFX = tankprefab.shootVFX;
        impactVFX = tankprefab.impactVFX;
        healthVFX = tankprefab.healthbar;
        
        print(activeMaterial);
        changeMaterial(activeMaterial);
        ShootDamage = tankprefab.DamageAmt;
        
        maxHealth = tankprefab.maxHealth;
        Health = maxHealth;

        projectileSpeed = tankprefab.shootSpeed;
        aimSpeed = tankprefab.aimSpeed;
        ApplyColour();
        
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
                if (collision.gameObject.GetComponent<Tank_Manager>())
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
                }
                else if (collision.gameObject.GetComponent<Bot_Tank_Manager>())
                {
                    Bot_Tank_Manager tankScript = collision.gameObject.GetComponent<Bot_Tank_Manager>();
                    if (tankScript._team != _team) //if not on my team
                    {
                        if (tankScript.canShoot == true) //if they have the bullet
                        {
                            CinemachineShake.Instance.shakeCam(ShakeIntensity, ShakeTime);
                            tankScript.canShoot = false;
                            Projectile.Instance.Bounce(collision.transform);
                        }
                        tankScript.TakeDamage(5);
                    }
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
            Die(PlayerSpawning.Randomised(_team));
        }
    }

    public void Die(int where)
    {
        if (canShoot) //if tank has the bullet
        {
            CinemachineShake.Instance.shakeCam(ShakeIntensity, ShakeTime);
            canShoot = false;
            Projectile.Instance.Bounce(transform);
        }
        freeze = true;
        if (currentTank != null)
        {
            currentTank.SetActive(false);
        }
        transform.position = gameManager.GetComponent<PlayerSpawning>().SpawnPoints[where].position;
        transform.eulerAngles = gameManager.GetComponent<PlayerSpawning>().SpawnPoints[where].eulerAngles;
        StartCoroutine(Respawn(2));
    }
    IEnumerator Respawn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        switch (_team)
        {
            case Team.team1:
                additionalAngle = 180f;
                break;
            case Team.team2:
                additionalAngle = 0f;
                break;
        }
        freeze = false;
        Health = maxHealth;
        if (currentTank != null)
        {
            currentTank.SetActive(true);
        }
    }
}
