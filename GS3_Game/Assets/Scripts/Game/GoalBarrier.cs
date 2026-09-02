using System.Collections;
using UnityEngine;

public class GoalBarrier : MonoBehaviour
{
    public Animator anim;
    public Team whichGoal;
    private float waitTime = 1f;

    private void Start()
    {
        if(anim == null)
        {
            Debug.LogWarning("Animator not assigned!");
        }
    }
    IEnumerator Opengoal(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        anim.SetBool("Open", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Tank"))
        {
            Team _team = other.gameObject.GetComponent<Tank_Manager>()._team;
            if(_team != whichGoal)
            {
                StartCoroutine(Opengoal(waitTime));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tank"))
        {
            Team _team = other.gameObject.GetComponent<Tank_Manager>()._team;
            if (_team != whichGoal)
            {
                StopCoroutine(Opengoal(waitTime));
                anim.SetBool("Open", false);
            }
        }
    }
}