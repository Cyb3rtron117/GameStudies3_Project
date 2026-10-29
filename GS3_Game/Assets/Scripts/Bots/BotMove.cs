using UnityEngine;
using UnityEngine.AI;

public class BotMove : MonoBehaviour
{
    public GameObject target;
    public NavMeshAgent agent;
    public GameObject turret;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        agent.SetDestination(target.transform.position);
        Vector3 desiredVelocity = agent.desiredVelocity;
        Vector3 targetVelocity = new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z);
        rb.linearVelocity = targetVelocity;

        if (desiredVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }

        agent.nextPosition = transform.position;


        Vector3 aimpos = target.transform.position - transform.position;
        aimpos.y = 0;
        turret.transform.eulerAngles = Quaternion.LookRotation(aimpos).eulerAngles;
    }
}
