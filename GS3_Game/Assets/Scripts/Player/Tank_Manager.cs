using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Tank_Manager : MonoBehaviour
{
    [SerializeField] private float Health = 100f;
    public float maxHealth = 100f;
    public bool isDead = false;
    public bool freeze = false;
    public int playerIndex;
    private bool isKeyboard = false;
    [SerializeField] private LayerMask MouseIncludeLayers = ~0;
    private List<GameObject> hitObjects = new List<GameObject>();
    [Header("Moving")]
    private Vector2 movementInput;
    private Vector2 aimInput;
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
    public float ShakeTime = 0.5f;
    public bool mgCanShoot = false;
    public float MaxMgShotAmt = 10;
    public float CurrentMgShotAmt = 10;
    [SerializeField] private float mgRechargeDelay = 2f;
    [SerializeField] private float mgRechargeInterval = 0.1f;
    private Coroutine rechargeCoroutine;

    [SerializeField] private float mgMaxDistance = 10f;
    [SerializeField] private LayerMask mgLayerMask = ~0;
    [SerializeField] private LineRenderer tracer;
    //[SerializeField] private float tracerDuration = 0.04f;
    private WaitForSeconds tracerDuration = new WaitForSeconds(0.04f);
    [SerializeField] private VisualEffect MGPingVFX;
    public float MGDamage = 1f;
    private Vector3 finalPoint = Vector3.zero;
    public AudioSource MGSound;

    [Header("UI")]
    public RawImage rocketItem;
    public Image[] rocketButton;
    public Image[] dashButton;
    public Image[] mgButton;
    public GameObject[] KeyboardUI;
    public GameObject[] ControllerUI;
    public Image dashBar;
    public Image mgBar;
    private bool drainDash = false;
    public Color inactiveColor;

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
    public GameObject Machinegun;
    public Transform lineShoot;

    [Header("Game Systems")]
    public GameObject gameManager;
    public Team _team; //set by PlayerSpawning
    public TextMeshProUGUI teamNumber;
    public TextMeshProUGUI countDown;
    public string controlScheme = " ";
    public ExplosionManager explodescript;
    public SoundManager soundScript;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineOrbitalFollow cineOrbit;
    public CinemachineCamera cineCam;
    public Camera mainCam;
    [SerializeField] private float BoostFOV = 70f;
    [SerializeField] private float RegularFOV = 60f;
    private bool zoomFOV = false;
    [SerializeField] private float zoomSpeed = 1f;

    private void OnEnable()
    {
        // Subscribe when enabled
        Countdown.UnfreezeAllPlayers += Unfreeze;
    }

    private void OnDisable()
    {
        // Always unsubscribe when disabled/destroyed
        Countdown.UnfreezeAllPlayers -= Unfreeze;
    }
    private void Start()
    {
        isDead = false;
        freeze = true;
        hitObjects.Clear();
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        cineOrbit = GetComponentInChildren<CinemachineOrbitalFollow>();
        cineCam.Lens.FieldOfView = RegularFOV;
        colours = gameManager.GetComponent<Colours>().colours;
        explodescript = gameManager.GetComponent<ExplosionManager>();
        soundScript = gameManager.GetComponent<SoundManager>();
        ChangeTank(tankIndex);
        CurrentMgShotAmt = MaxMgShotAmt;

        rocketItem.color = inactiveColor;

        rocketButton[0].color = inactiveColor;
        rocketButton[1].color = inactiveColor;
        dashButton[0].color = Color.white;
        dashButton[1].color = Color.white;

        foreach(var thing in KeyboardUI)
        {
            thing.SetActive(false);
        }
        foreach (var thing in ControllerUI)
        {
            thing.SetActive(false);
        }
        isBoosting = false;
        teamNumber.SetText($"You are {_team}");
        switch(_team)
        {
            case Team.team1:
                additionalAngle = 180f;
                break;
            case Team.team2:
                additionalAngle = 0f;
                break;
        }
        switch (controlScheme)
        {
            case "Mouse & Keyboard":
                foreach (var thing in KeyboardUI)
                {
                    thing.SetActive(true);
                }
                isKeyboard = true;
                break;
            case "Gamepad":
                foreach (var thing in ControllerUI)
                {
                    thing.SetActive(true);
                }
                isKeyboard = false;
                break;
        }
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        if(movementInput.sqrMagnitude>0)
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
    public void MGInput(InputAction.CallbackContext context)
    {
        if (!freeze && !canShoot && !Scoring.GameEnd && !isDead)
        {
            if (context.performed)
            {
                if (rechargeCoroutine != null)
                {
                    StopCoroutine(rechargeCoroutine);
                    rechargeCoroutine = null;
                }
                if(CurrentMgShotAmt>0)
                {
                    MGSound.Play();
                }
                
                mgCanShoot = true;
                Machinegun.GetComponent<Animator>().SetBool("Shoot", mgCanShoot);
                InvokeRepeating(nameof(MGShot), 0.1f, 0.1f);
            }
            else if (context.canceled)
            {
                MGSound.Stop();
                if (rechargeCoroutine == null)
                {
                    rechargeCoroutine = StartCoroutine(ReloadMG());
                }
                mgCanShoot = false;
                Machinegun.GetComponent<Animator>().SetBool("Shoot", mgCanShoot);
                CancelInvoke(nameof(MGShot));
                tracer.enabled = false;
            }
        }
        else
        {
            MGSound.Stop();
            mgCanShoot = false;
            Machinegun.GetComponent<Animator>().SetBool("Shoot", mgCanShoot);
            CancelInvoke(nameof(MGShot));
            tracer.enabled = false;

            if(context.performed)
            {
                soundScript.PlayError();
            }
        }
    }
    public void AimInput(InputAction.CallbackContext context)
    {
        if (!freeze && !Scoring.GameEnd && !isDead)
        {
            aimInput = context.ReadValue<Vector2>();
        }
    }
    public void BoostInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isBoosting && !freeze && !Scoring.GameEnd && !isDead)
            {
                isBoosting = true;
                StartCoroutine(Boost(boostTime));
            }
            else
            {
                soundScript.PlayError();
            }
        }
    }
    private void Move()
    {
        if(!freeze && !Scoring.GameEnd && !isDead)
        {
            //rb.linearVelocity = transform.forward * movement.y * currentSpeed;
            //REGULAR   
            //transform.Rotate(Vector3.up * movement.x * turnSpeed * Time.fixedDeltaTime);

            //REVERSE INVERSE
            /*
            if (movement.y < 0)
            {
                transform.Rotate(Vector3.up * movement.x * -turnSpeed * Time.fixedDeltaTime);
            }
            else
            {
                transform.Rotate(Vector3.up * movement.x * turnSpeed * Time.fixedDeltaTime);
            }*/

            Vector3 movedir = new Vector3(movementInput.x, 0, movementInput.y) * MoveMultiplier;
            if (movedir.sqrMagnitude < 0.01f)
            {
                rb.linearVelocity = Vector3.zero;
                return;
            }  
            // The tank's CURRENT forward direction
            Vector3 forwardMovement = rb.rotation * Vector3.forward;
            

            // Signed angle to the desired world-space direction
            float angle = Vector3.SignedAngle(forwardMovement, movedir, Vector3.up);

            // Reverse if the target is behind the tank
            float moveSign = Mathf.Abs(angle) > 100f ? -1f : 1f;

            // Move along current forward, or backward when reversing
            Vector3 _movement = forwardMovement * currentSpeed * moveSign;

            Vector3 velocity = rb.linearVelocity;
            velocity.x = _movement.x;
            velocity.z = _movement.z;
            rb.linearVelocity = velocity;

            // When reversing, face the opposite direction from the target
            Vector3 facingDirection = moveSign < 0f
                ? -movedir
                : movedir;

            Quaternion targetRotation = Quaternion.LookRotation(facingDirection);

            Quaternion smoothRotation = Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    turnSpeed * Time.fixedDeltaTime
                );

            rb.MoveRotation(smoothRotation);

        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    public void HasProjectile()
    {
        canShoot = true;
        rocketItem.color = Color.green;
        rocketButton[0].color = Color.white;
        rocketButton[1].color = Color.white;
    }
    public void LoseProjectile()
    {
        Debug.Log($"Player tank {playerIndex} lost projectile. \n The priority gameobject was: {Projectile.priorityObject}");
        canShoot = false;
        rocketItem.color = inactiveColor;
        rocketButton[0].color = inactiveColor;
        rocketButton[1].color = inactiveColor;
        Projectile.Instance.Bounce(transform);
    }
    private void Shoot()
    {
        if (canShoot && !freeze && !Scoring.GameEnd)
        {
            anim.SetTrigger("Shoot");
            canShoot = false;
            soundScript.PlayShoot();
            cooldownTime = shootCooldown;
            CinemachineShake.Instance.ShakeCam(ShakeType.Shoot, ShakeTime);
            Vector3 shootDir = new Vector3(0f,turret.eulerAngles.y, 0f);
            Projectile.Instance.Shoot(barrel, shootDir, gameObject, _team, ShootDamage, projectileSpeed);
            rocketItem.color = inactiveColor;
            rocketButton[0].color = inactiveColor;
            rocketButton[1].color = inactiveColor;

            shootVFX.Play();
        }
        else
        {
            soundScript.PlayError();
        }
    }
    public void MGShot()
    {
        if (freeze || canShoot || isDead || Scoring.GameEnd || CurrentMgShotAmt <= 0)
        {
            if (CurrentMgShotAmt <= 0)
            {
                CancelInvoke(nameof(MGShot));
                mgCanShoot = false;
                Machinegun.GetComponent<Animator>().SetBool("Shoot", false);
                tracer.enabled = false;

                if (rechargeCoroutine == null)
                {
                    rechargeCoroutine = StartCoroutine(ReloadMG());
                }
            }
            if(MGSound.isPlaying)
            {
                MGSound.Stop();
            }
            return;
        }
        CurrentMgShotAmt--;

        //print("mg shooting");
        Vector3 startPos = turret.position;
        Vector3 endPos = startPos + turret.forward * mgMaxDistance;

        finalPoint = endPos;

        if (Physics.Linecast(startPos, endPos, out RaycastHit hit, mgLayerMask))
        {
            finalPoint = hit.point;
            //print($"Machine gun hit {hit.collider.name}. Parent is {hit.collider.transform.parent}");
            if(hit.collider.GetComponent<Tank_Manager>())
            {
                soundScript.PlayPing();
                Tank_Manager tankScript = hit.collider.GetComponent<Tank_Manager>();
                
                Debug.Log($"MG HIT PLAYER: {tankScript.name}");
                /*
                Debug.Log($"Shooter team: {_team} | Target team: {tankScript._team}");
                Debug.Log($"Target isDead: {tankScript.isDead}");
                */
                if (tankScript._team != _team) //if not on my team
                {
                    tankScript.TakeDamage(MGDamage);
                }
            }
            else if (hit.collider.GetComponent<Bot_Tank_Manager>())
            {
                soundScript.PlayPing();
                Bot_Tank_Manager bottankScript = hit.collider.GetComponent<Bot_Tank_Manager>();
                
                Debug.Log($"MG HIT PLAYER: {bottankScript.name}");
                Debug.Log($"Shooter team: {_team} | Target team: {bottankScript._team}");
                if (bottankScript._team != _team) //if not on my team
                {
                    bottankScript.TakeDamage(MGDamage);
                }
            }
        }
        MGPingVFX.SetVector3("Position", finalPoint);
        MGPingVFX.Play();
        StartCoroutine(ShowTracer());
    }
    private IEnumerator ShowTracer()
    {

        tracer.enabled = true;

        yield return tracerDuration;

        tracer.enabled = false;
    }
    private IEnumerator ReloadMG()
    {
        yield return new WaitForSeconds(mgRechargeDelay);

        while (CurrentMgShotAmt < MaxMgShotAmt)
        {
            CurrentMgShotAmt++;

            yield return new WaitForSeconds(mgRechargeInterval);
        }

        rechargeCoroutine = null;
    }
    IEnumerator Boost(float waitTime)
    {
        zoomFOV = true;
        currentSpeed = boostSpeed;
        drainDash = true;
        dashButton[0].color = inactiveColor;
        dashButton[1].color = inactiveColor;
        soundScript.PlayWoosh();

        yield return new WaitForSeconds(waitTime);
        drainDash = false;
        currentSpeed = moveSpeed;
        zoomFOV = false;
        StartCoroutine(BoostCooldown(boostCooldown));
    }
    IEnumerator BoostCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isBoosting = false;
        dashButton[0].color = Color.white;
        dashButton[1].color = Color.white;
        hitObjects.Clear();
    }
    private void RotateTurret()
    {
        if (freeze || isDead || Scoring.GameEnd)
        {
            turret.rotation = Quaternion.Euler(0, additionalAngle, 0);
        }
        else
        {
            if (!isKeyboard)
            {
                Vector3 aimDir = new Vector3(aimInput.x, 0, aimInput.y);
                //additionalAngle += aimInput * aimSpeed;
                if (aimDir.sqrMagnitude > 0.001f)
                {
                    float yRotation = Quaternion.LookRotation(aimDir).eulerAngles.y + additionalAngle;
                    turret.rotation = Quaternion.Euler(0, yRotation, 0);
                }
            }
            else
            {
                Vector2 mousePosition = aimInput;

                Vector3 viewportPosition = mainCam.ScreenToViewportPoint(mousePosition);
                if (viewportPosition.x >= 0f && viewportPosition.x <= 1f && viewportPosition.y >= 0f && viewportPosition.y <= 1f)
                {
                    Ray ray = mainCam.ViewportPointToRay(viewportPosition);

                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f, MouseIncludeLayers))
                    {
                        Vector3 difference = hit.point - turret.position;
                        difference.y = 0f;
                        if (difference.sqrMagnitude > 0.001f)
                        {
                            float yRotation = Quaternion.LookRotation(difference).eulerAngles.y;
                            turret.rotation = Quaternion.Euler(0, yRotation, 0);
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (Countdown.count)
        {
            countDown.SetText(Countdown.Counter);
            if (Countdown.number <= 0)
            {
                countDown.SetText(" ");
                freeze = false;
            }
            else if (Countdown.number <= 0.5)
            {
                countDown.SetText("Go!");
            }
        }

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

        mgBar.fillAmount = CurrentMgShotAmt / MaxMgShotAmt;

        if(!freeze && !canShoot && !isDead && !Scoring.GameEnd && CurrentMgShotAmt > 0)
        {
            mgButton[0].color = Color.white;
            mgButton[1].color = Color.white;

        }
        else
        {
            mgButton[0].color = inactiveColor;
            mgButton[1].color = inactiveColor;
        }

        CamZoom(zoomFOV);
        
        if (drainDash && dashBar.fillAmount > 0)
        {
            dashBar.fillAmount -= (1/boostTime) * Time.fixedDeltaTime;
        }
        else if(!drainDash && dashBar.fillAmount < 1)
        {
            dashBar.fillAmount += (1/boostCooldown) * Time.fixedDeltaTime;
        }
    }
    private void LateUpdate()
    {
        tracer.SetPosition(0, lineShoot.position);
        tracer.SetPosition(1, finalPoint);
    }
    private void CamZoom(bool zoom)
    {
        if (zoom && cineCam.Lens.FieldOfView < BoostFOV)
        {
            cineCam.Lens.FieldOfView = Mathf.Lerp(cineCam.Lens.FieldOfView, BoostFOV, Time.fixedDeltaTime * zoomSpeed);
        }
        else if (!zoom && cineCam.Lens.FieldOfView > RegularFOV)
        {
            cineCam.Lens.FieldOfView = Mathf.Lerp(cineCam.Lens.FieldOfView, RegularFOV, Time.fixedDeltaTime * zoomSpeed);
        }
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
        Machinegun = tankprefab.Machinegun;
        lineShoot = tankprefab.LineShoot;
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
                if (collision.gameObject.GetComponent<Tank_Manager>() && !hitObjects.Contains(collision.gameObject))
                {
                    hitObjects.Add(collision.gameObject);
                    Tank_Manager tankScript = collision.gameObject.GetComponent<Tank_Manager>();
                    if (tankScript._team != _team) //if not on my team
                    {
                        if (tankScript.canShoot == true && Projectile.priorityObject == collision.gameObject) //if they have the bullet
                        {
                            CinemachineShake.Instance.ShakeCam(ShakeType.Ram, ShakeTime);
                            tankScript.LoseProjectile();
                        }
                        tankScript.TakeDamage(5);
                    }
                }
                else if (collision.gameObject.GetComponent<Bot_Tank_Manager>() && !hitObjects.Contains(collision.gameObject))
                {
                    Bot_Tank_Manager tankScript = collision.gameObject.GetComponent<Bot_Tank_Manager>();
                    if (tankScript._team != _team) //if not on my team
                    {
                        if (tankScript.canShoot == true && Projectile.priorityObject == collision.gameObject) //if they have the bullet
                        {
                            CinemachineShake.Instance.ShakeCam(ShakeType.Ram, ShakeTime);
                            tankScript.LoseProjectile();
                        }
                        tankScript.TakeDamage(5);
                    }
                }
                impactVFX.Play();
                soundScript.PlaySmash();

            }
        }
    }
    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }
        //print("damaged");
        Health -= amount;
        if(Health <= 0)
        {
            isDead = true;
            Die(PlayerSpawning.Randomised(_team));
        }
    }

    public void Die(int where)
    {
        explodescript.PlayerExplode(playerIndex, transform.position);
        Debug.Log($"Player died. Current priority: {Projectile.priorityObject}");
        if (Projectile.priorityObject == gameObject) //if tank has the bullet
        {
            Projectile.Instance.Bounce(transform);
        }
        Debug.Log($"Priority after death: {Projectile.priorityObject}");


        CinemachineShake.Instance.ShakeCam(ShakeType.Dead, ShakeTime);
        
        mgCanShoot = false;
        Machinegun.GetComponent<Animator>().SetBool("Shoot", mgCanShoot);
        CancelInvoke(nameof(MGShot));
        tracer.enabled = false;
        rocketItem.color = inactiveColor;
        rocketButton[0].color = inactiveColor;
        rocketButton[1].color = inactiveColor;
        freeze = true;
        if (currentTank != null)
        {
            currentTank.SetActive(false);
        }
        transform.position = gameManager.GetComponent<PlayerSpawning>().SpawnPoints[where].position;
        transform.eulerAngles = gameManager.GetComponent<PlayerSpawning>().SpawnPoints[where].eulerAngles;
        canShoot = false;
        soundScript.PlayPlayerExpl();
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
        hitObjects.Clear();
        if(!Countdown.count && !Countdown.freeze && !Scoring.GameEnd)
        {
            Unfreeze();
        }
        else
        {
            freeze = true;
        }
        CurrentMgShotAmt = MaxMgShotAmt;
        Health = maxHealth;
        isDead = false;
        if (currentTank != null)
        {
            currentTank.SetActive(true);
        }
        soundScript.PlayRespawn();
    }
    private void Unfreeze()
    {
        freeze = false;
    }
}
