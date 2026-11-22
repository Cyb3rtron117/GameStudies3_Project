using UnityEngine;

public class SplitscreenDivider : MonoBehaviour
{
    [SerializeField] private GameObject verticalLine;
    [SerializeField] private GameObject horizontalLine;
    public static int PlayerAmt = 0;
    private void Start()
    {
        if(PlayerAmt >=2) //Vertical Line
        {
            verticalLine.SetActive(true);
        }
        if (PlayerAmt >= 3) //Horizontal Line
        {
            horizontalLine.SetActive(true);
        }
    }
}