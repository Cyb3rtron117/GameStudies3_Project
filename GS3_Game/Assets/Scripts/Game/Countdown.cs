using System.Collections;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public static string Counter = "Start";
    public static float number = 3.1f;
    public static bool count = false;
    private void Start()
    {
        number = 3.1f;
        count = false;
        startCountDown();
    }
    public void startCountDown()
    {
        number = 3.1f;
        count = false;
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        number = 3.1f;
        yield return new WaitForSeconds(1f);
        count = true;
    }
    private void FixedUpdate()
    {
        if (count)
        {
            if (number > 0)
            {
                number -= Time.fixedDeltaTime;
                Counter = number.ToString("F0");
            }
            else
            {
                count = false;
            }
        }
    }
}
