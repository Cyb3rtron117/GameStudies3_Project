using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    public static Projectile Instance { get; private set; }
    private Rigidbody rb;
    public float bulletSpeed = 1f;
    public float bounceForce = 10f;
    [SerializeField] private float spinSpeed = 1f;
    [SerializeField] private float fallMultiplier = 2f;
    [SerializeField] private BulletMode mode;
    [SerializeField] private GameObject _shooter;
    public static GameObject priorityObject;
    [SerializeField] private GameObject _priorityObj;

    [SerializeField] private Transform FreezePos;
    private MeshRenderer renderer;
    [SerializeField] private BoxCollider triggerZone;
    [SerializeField] private Material defaultMat;
    [SerializeField] private Team _team;
    [SerializeField] private Scoring scoreScript;
    [SerializeField] private bool ignoreCollisions = false;
    [SerializeField] private VisualEffect impactVFX;
    [SerializeField] private VisualEffect smokeVFX;
    [SerializeField] private VisualEffect Indicator;
    [SerializeField] private VisualEffect FallIndicator;
    private Vector3 fallpos;
    private Vector3 bulletForward;

    private Coroutine bounceCoroutine;
    private bool isBouncing = false;

    public float Damage;
    public SoundManager soundScript;
    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        UpdatePriority(this.gameObject);
        smokeVFX = GetComponentInChildren<VisualEffect>();
        Damage = 0;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            scoreScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Scoring>();
            if(impactVFX == null)
            {
                impactVFX = GameObject.FindGameObjectWithTag("ImpactVFX").GetComponent<VisualEffect>();
            }
            
        }
        ResetBullet();

    }
    private void FixedUpdate()
    {
        switch (mode)
        {
            case BulletMode.Frozen:
                transform.localEulerAngles = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                break;

            case BulletMode.Move:
                transform.localEulerAngles = bulletForward;
                rb.linearVelocity = transform.forward * bulletSpeed;
                rb.angularVelocity = Vector3.zero;
                break;

            case BulletMode.Display:
                transform.Rotate(Vector3.up * spinSpeed * Time.fixedDeltaTime);
                break;
            case BulletMode.Fall:
                fallpos = new Vector3(transform.position.x, 0, transform.position.z);
                FallIndicator.transform.position = fallpos;
                transform.Rotate(Vector3.up * spinSpeed * Time.fixedDeltaTime);
                if(rb.linearVelocity.y <= 0)
                {
                    rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
                }
                break;
            case BulletMode.Bounce:
                /*
                if (mode == BulletMode.Bounce)
                {
                    Debug.Log($"Actual bounce velocity: {rb.linearVelocity}");
                }
                */
                fallpos = new Vector3(transform.position.x, 0, transform.position.z);
                FallIndicator.transform.position = fallpos;
                transform.Rotate(Vector3.up * spinSpeed * Time.fixedDeltaTime);
                break;
        }
        if(Indicator != null)
        {
            if(priorityObject == this.gameObject)
            {
                Indicator.SetFloat("Offset", 1f);
                Indicator.SetVector3("Position", priorityObject.transform.position);
            }
            else
            {
                Indicator.SetFloat("Offset", 3.8f);
                Indicator.SetVector3("Position", priorityObject.transform.position);
            }
        }
    }
    public void Shoot(Transform shootPos, Vector3 shootRot, GameObject Shooter, Team team, float damage, float speed)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        _shooter = Shooter;
        transform.localEulerAngles = shootRot;
        bulletForward = shootRot;
        transform.position = shootPos.position;
        _team = team;
        Damage = damage;
        bulletSpeed = speed;
        smokeVFX.Play();
        mode = BulletMode.Move;
        triggerZone.enabled = true;
        UpdatePriority(this.gameObject);
    }
    private void Freeze()
    {
        triggerZone.enabled = false;
        bulletForward = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = FreezePos.position;
        transform.localEulerAngles = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(ignoreCollisions || Scoring.GameEnd)
        {
            return;
        }
        else
        {
            FallIndicator.transform.position = -Vector3.up;
            FallIndicator.enabled = false;
            if (other.gameObject.CompareTag("Arena") && (mode != BulletMode.Display) && (mode != BulletMode.Bounce))
            {
                placeVFX();
                goDisplay();
                smokeVFX.Stop();
            }
            if (other.gameObject.CompareTag("Tank") && mode != BulletMode.Frozen && (mode != BulletMode.Bounce) && (mode != BulletMode.Fall))
            {
                rb.useGravity = false;
                if (_shooter != null && other.gameObject == _shooter) //if it is the shooter, if there is one, ignore it
                {
                    return;
                }
                Tank_Manager player = other.GetComponent<Tank_Manager>();
                Bot_Tank_Manager bot = other.GetComponent<Bot_Tank_Manager>();

                if (player == null && bot == null)
                    return;

                // Don't give the projectile to a dead player.
                if (player != null && player.isDead)
                {
                    return;
                }

                else if (bot != null && bot.isDead)
                {
                    return;
                }
                    
                placeVFX();

                float damageToApply = Damage;

                GoHide();
                UpdatePriority(other.gameObject);

                Damage = 0f;
                smokeVFX.Stop();

                if (player != null)
                {
                    player.HasProjectile();
                    SwapColour(player.activeMaterial);
                    player.TakeDamage(damageToApply);
                }
                else if (bot != null)
                {
                    bot.HasProjectile();
                    SwapColour(bot.activeMaterial);
                    bot.TakeDamage(damageToApply);
                }
            }
            CinemachineShake.Instance.ShakeCam(ShakeType.BulletHit, 0.5f);
            if (other.gameObject.CompareTag("Goal1") && mode != BulletMode.Frozen && (mode != BulletMode.Bounce) && (mode != BulletMode.Fall))
            {
                print($"{Team.team2} got a point");
                scoreScript.Score(Team.team2);
                UpdatePriority(this.gameObject);
                ResetBullet();
                CinemachineShake.Instance.ShakeCam(ShakeType.Goal, 1);
                soundScript.PlayBoom();
            }

            if (other.gameObject.CompareTag("Goal2") && mode != BulletMode.Frozen && (mode != BulletMode.Bounce) && (mode != BulletMode.Fall))
            {
                print($"{Team.team1} got a point");
                scoreScript.Score(Team.team1);
                UpdatePriority(this.gameObject);
                ResetBullet();
                CinemachineShake.Instance.ShakeCam(ShakeType.Goal, 1);
                soundScript.PlayBoom();
            }
            soundScript.PlayMissileImp();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") && mode != BulletMode.Bounce && mode != BulletMode.Display)
        {
            goDisplay();
            CinemachineShake.Instance.ShakeCam(ShakeType.BulletHit, 0.5f);
            soundScript.PlayMissileImp();
        }
    }
    private void SwapColour(Material newMat)
    {
        Material[] materials = renderer.materials;
        materials[0] = newMat;
        renderer.materials = materials;
    }

    public void GoHide()
    {
        FallIndicator.transform.position = -Vector3.up;
        FallIndicator.enabled = false;
        triggerZone.enabled = false;
        mode = BulletMode.Frozen;
        transform.eulerAngles = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        Freeze();
        _shooter = null;
        _team = Team.none;
        bulletForward = Vector3.zero;
    }
    private void goDisplay()
    {
        FallIndicator.transform.position = -Vector3.up;
        FallIndicator.enabled = false;
        UpdatePriority(this.gameObject);
        Damage = 0f;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        mode = BulletMode.Display;
        _shooter = null;
        _team = Team.none;
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        bulletForward = Vector3.zero;
        
        ignoreCollisions = false;
        triggerZone.enabled = true;
    }
    public void Bounce(Transform bouncePos)
    {
        if (isBouncing)
            return;

        isBouncing = true;
        triggerZone.enabled = false;
        ignoreCollisions = true;
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
            bounceCoroutine = null;
        }
        mode = BulletMode.Bounce;
        fallpos = new Vector3(transform.position.x, 0, transform.position.z);
        FallIndicator.transform.position = fallpos;
        FallIndicator.enabled = true;
        UpdatePriority(this.gameObject);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.rotation = Quaternion.identity;
        transform.position = bouncePos.position;

        bulletForward = Vector3.zero;

        rb.useGravity = true;
        rb.WakeUp();
        soundScript.PlayWoosh();
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);

        //Debug.Log($"Bounce velocity: {rb.linearVelocity}");
        
        bounceCoroutine = StartCoroutine(startFall());
    }
    private IEnumerator enableTankCollision()
    {
        yield return new WaitForSeconds(1f);
        isBouncing = false;
        mode = BulletMode.Fall;
        ignoreCollisions = false;
        triggerZone.enabled = true;

        bounceCoroutine = null;
    }
    private IEnumerator startFall()
    {
        yield return new WaitForSeconds(1f);
        isBouncing = false;
        mode = BulletMode.Fall;
        bounceCoroutine = null;
    }
    private void ResetBullet()
    {
        _shooter = null;
        smokeVFX.Stop();
        Damage = 0f;
        rb.useGravity = false;
        SwapColour(defaultMat);
        transform.position = new Vector3(0, 1, 0);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        goDisplay();
        ignoreCollisions = false;
        bulletForward = Vector3.zero;
        StartCoroutine(recallVFX());
        UpdatePriority(this.gameObject);
        FallIndicator.transform.position = -Vector3.up;
        FallIndicator.enabled = false;
    }
    private void placeVFX()
    {
        impactVFX.Stop();
        impactVFX.gameObject.transform.position = transform.position;
        impactVFX.gameObject.transform.eulerAngles = transform.eulerAngles;
        impactVFX.Play();
        StartCoroutine(recallVFX());
    }
    private IEnumerator recallVFX()
    {
        print("vfx recalled");
        yield return new WaitForSeconds(1.8f); //the decal dies at 1.72 seconds
        impactVFX.gameObject.transform.position = transform.position;
        impactVFX.gameObject.transform.eulerAngles = transform.eulerAngles;
    }
    public void UpdatePriority(GameObject whichObject)
    {
        /*
        if(whichObject == null)
        {
            priorityObject = this.gameObject;
        }
        else
        {
            priorityObject = whichObject;
        }
        _priorityObj = priorityObject;
        */
        GameObject oldPriority = priorityObject;

        priorityObject = whichObject != null ? whichObject : gameObject;
        _priorityObj = priorityObject;

        
        Debug.Log(
            $"PROJECTILE PRIORITY CHANGED\n" +
            $"Old: {(oldPriority != null ? oldPriority.name : "NULL")}\n" +
            $"New: {priorityObject.name}\n" +
            $"Time: {Time.time}\n" +
            $"CALL STACK:\n{System.Environment.StackTrace}"
        );
        
    }
}

public enum BulletMode
{
    Frozen,
    Display, 
    Move,
    Fall,
    Bounce
}

public enum Team
{
    team1,
    team2,
    none
}