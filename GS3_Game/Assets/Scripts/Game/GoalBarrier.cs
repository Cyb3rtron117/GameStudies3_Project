using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBarrier : MonoBehaviour
{
    public Animator anim;
    public Team whichGoal;
    private float waitTime = 1f;
    private List<GameObject> enemTeam = new List<GameObject>(2);

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
        if (enemTeam.Count > 0)
        {
            anim.SetBool("Open", true);
        }
        else if (enemTeam.Count == 0)
        {
            anim.SetBool("Open", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Tank"))
        {
            Team _team = other.gameObject.GetComponent<Tank_Manager>()._team;
            if(_team != whichGoal)
            {
                if(!enemTeam.Contains(other.gameObject))
                {
                    enemTeam.Add(other.gameObject);
                }
                updateBarrier();
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
                if (enemTeam.Contains(other.gameObject))
                {
                    enemTeam.Remove(other.gameObject);
                }
                updateBarrier();
            }
        }
    }

    private void updateBarrier()
    {
        if(enemTeam.Count > 0)
        {
            StartCoroutine(Opengoal(waitTime));
        }
        else if (enemTeam.Count == 0)
        {
            StopCoroutine(Opengoal(waitTime));
            anim.SetBool("Open", false);
        }
    }
}