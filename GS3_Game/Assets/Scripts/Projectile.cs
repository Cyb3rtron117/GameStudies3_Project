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

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
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
                rb.linearVelocity = Vector3.zero;
                transform.Rotate(Vector3.up * spinSpeed * Time.fixedDeltaTime);
                break;
        }
    }
    public void Shoot(Transform shootPos, Vector3 shootRot, GameObject Shooter)
    {
        _shooter = Shooter;
        transform.localEulerAngles = shootRot;
        transform.position = shootPos.position;
        mode = BulletMode.Move;
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
            mode = BulletMode.Display;
            _shooter = null;
        }
        if (other.gameObject.CompareTag("Tank"))
        {
            if(_shooter != null)
            {
                if (other.gameObject != _shooter)
                {
                    mode = BulletMode.Frozen;
                    Freeze();
                    _shooter = null;
                    other.gameObject.GetComponent<Tank_Manager>().canShoot = true;
                    SwapColour(other.gameObject.GetComponent<Tank_Manager>().activeMaterial);
                }
            }
            else
            {
                mode = BulletMode.Frozen;
                Freeze();
                _shooter = null;
                other.gameObject.GetComponent<Tank_Manager>().canShoot = true;
                SwapColour(other.gameObject.GetComponent<Tank_Manager>().activeMaterial);
            }
           
        }
    }
    private void SwapColour(Material newMat)
    {
        Material[] materials = renderer.materials;
        materials[0] = newMat;
        renderer.materials = materials;
    }
}

public enum BulletMode
{
    Frozen,
    Display, 
    Move
}

