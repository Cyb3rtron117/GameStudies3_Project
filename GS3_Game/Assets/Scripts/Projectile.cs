using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Projectile Instance { get; private set; }
    private Rigidbody rb;
    public float bulletSpeed = 1f;
    [SerializeField] private float spinSpeed = 1f;
    [SerializeField] private BulletMode mode;
    [SerializeField] private GameObject _shooter;

    [SerializeField] private Transform FreezePos;
    private MeshRenderer renderer;
    [SerializeField] private Material defaultMat;
    [SerializeField] private Team _team;
    [SerializeField] private Scoring scoreScript;
    [SerializeField] private PlayerSpawning spawnScript;

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
                break;

            case BulletMode.Move:
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
        transform.position = shootPos.position;
        mode = BulletMode.Move;
        _team = team;
    }
    private void Freeze()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = FreezePos.position;
        transform.rotation = FreezePos.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Arena"))
        {
            goDisplay();
        }
        if (other.gameObject.CompareTag("Tank"))
        {
            if(_shooter != null)
            {
                if (other.gameObject != _shooter)
                {
                    GoHide();
                    other.gameObject.GetComponent<Tank_Manager>().canShoot = true;
                    SwapColour(other.gameObject.GetComponent<Tank_Manager>().activeMaterial);
                }
            }
            else
            {
                GoHide();
                other.gameObject.GetComponent<Tank_Manager>().canShoot = true;
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
    private void SwapColour(Material newMat)
    {
        Material[] materials = renderer.materials;
        materials[0] = newMat;
        renderer.materials = materials;
    }

    public void GoHide()
    {
        mode = BulletMode.Frozen;
        Freeze();
        _shooter = null;
        _team = Team.none;
    }
    private void goDisplay()
    {
        rb.linearVelocity = Vector3.zero;
        mode = BulletMode.Display;
        _shooter = null;
        _team = Team.none;
    }
    private void Reset()
    {
        SwapColour(defaultMat);
        transform.position = new Vector3(0, 1, 0);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        goDisplay();
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