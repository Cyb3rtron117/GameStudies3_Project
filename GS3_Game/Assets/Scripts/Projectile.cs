using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public static Projectile Instance { get; private set; }
    private Rigidbody rb;
    public float bulletSpeed = 1f;
    public float bounceForce = 10f;
    [SerializeField] private float spinSpeed = 1f;
    [SerializeField] private BulletMode mode;
    [SerializeField] private GameObject _shooter;

    [SerializeField] private Transform FreezePos;
    private MeshRenderer renderer;
    [SerializeField] private Material defaultMat;
    [SerializeField] private Team _team;
    [SerializeField] private Scoring scoreScript;
    [SerializeField] private PlayerSpawning spawnScript;
    [SerializeField] private bool ignoreCollisions = false;
    private Vector3 bulletForward;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        scoreScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Scoring>();
        spawnScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerSpawning>();
        Reset();

    }
    private void FixedUpdate()
    {
        switch (mode)
        {
            case BulletMode.Frozen:
                transform.localEulerAngles = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
                break;

            case BulletMode.Move:
                transform.localEulerAngles = bulletForward;
                rb.linearVelocity = transform.forward * bulletSpeed;
                break;

            case BulletMode.Display:
                transform.Rotate(Vector3.up * spinSpeed * Time.fixedDeltaTime);
                break;
        }
    }
    public void Shoot(Transform shootPos, Vector3 shootRot, GameObject Shooter, Team team)
    {
        _shooter = Shooter;
        transform.localEulerAngles = shootRot;
        bulletForward = shootRot;
        transform.position = shootPos.position;
        mode = BulletMode.Move;
        _team = team;
    }
    private void Freeze()
    {
        bulletForward = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        transform.position = FreezePos.position;
        transform.localEulerAngles = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Arena") && !ignoreCollisions)
        {
            goDisplay();
        }
        if (other.gameObject.CompareTag("Tank") && !ignoreCollisions)
        {
            rb.useGravity = false;
            if(_shooter != null)
            {
                if (other.gameObject != _shooter)
                {
                    GoHide();
                    other.gameObject.GetComponent<Tank_Manager>().HasProjectile();
                    SwapColour(other.gameObject.GetComponent<Tank_Manager>().activeMaterial);
                }
            }
            else
            {
                GoHide();
                other.gameObject.GetComponent<Tank_Manager>().HasProjectile();
                SwapColour(other.gameObject.GetComponent<Tank_Manager>().activeMaterial);
            }
        }

        if (other.gameObject.CompareTag("Goal1"))
        {
            print($"{Team.team2} got a point");
            scoreScript.Score(Team.team2);
            spawnScript.RespawnPlayers();
            Reset();
        }

        if (other.gameObject.CompareTag("Goal2"))
        {
            print($"{Team.team1} got a point");
            scoreScript.Score(Team.team1);
            spawnScript.RespawnPlayers();
            Reset();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            goDisplay();
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
        transform.eulerAngles = Vector3.zero;
        rb.useGravity = false;
        mode = BulletMode.Frozen;
        Freeze();
        _shooter = null;
        _team = Team.none;
        bulletForward = Vector3.zero;
    }
    private void goDisplay()
    {
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        mode = BulletMode.Display;
        _shooter = null;
        _team = Team.none;
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        bulletForward = Vector3.zero;
    }
    public void Bounce(Transform bouncePos)
    {
        print("bounce");
        ignoreCollisions = true;
        transform.position = bouncePos.position;
        rb.useGravity = true;
        rb.AddForce(transform.up * bounceForce, ForceMode.Impulse);
        mode = BulletMode.Display;
        StartCoroutine(enableTankCollision());
        bulletForward = Vector3.zero;
    }
    private IEnumerator enableTankCollision()
    {
        print("collisions enabled");
        yield return new WaitForSeconds(0.5f);
        ignoreCollisions = false;
    }
    private void Reset()
    {
        rb.useGravity = false;
        SwapColour(defaultMat);
        transform.position = new Vector3(0, 1, 0);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        goDisplay();
        ignoreCollisions = false;
        bulletForward = Vector3.zero;
    }
    
}

public enum BulletMode
{
    Frozen,
    Display, 
    Move
}

public enum Team
{
    team1,
    team2,
    none
}