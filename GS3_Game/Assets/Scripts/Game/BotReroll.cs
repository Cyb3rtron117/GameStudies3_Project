using System.Collections;
using UnityEngine;

public class BotReroll : MonoBehaviour
{
    private float rerollTime = 1f;
    private WaitForSeconds _wait = new WaitForSeconds(0.1f);
    private WaitForSeconds additional_wait = new WaitForSeconds(1f);
    private bool _isRunning;
    public Menu_Tank tankScript;
    private int selectedTank;
    public void StartLoop(float duration)
    {
        if (!_isRunning)
        {
            selectedTank = Random.Range(0, 4);
            rerollTime += duration;
            _isRunning = true;
            StartCoroutine(RunEverySecond());
            StartCoroutine(StopLoop());
        }
    }

    private IEnumerator StopLoop()
    {
        yield return new WaitForSeconds(rerollTime);
        _isRunning = false;
        tankScript.ChangeTank(selectedTank);
        StartCoroutine(ready());
    }
    private IEnumerator ready()
    {
        yield return additional_wait;
        tankScript.ReadyAction();
    }

    private IEnumerator RunEverySecond()
    {
        while (_isRunning)
        {
            MyMethod();
            yield return _wait;
        }
    }

    private void MyMethod() //runs every 0.1s
    {
        tankScript.NextTank();
    }
}
