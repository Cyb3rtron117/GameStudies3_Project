using UnityEngine;

public class tankButtons : MonoBehaviour
{
    public GameObject[] tankStats;
    public GameObject[] swapButtons;
    public string playerControlscheme = "";
    private void Start()
    {
        foreach (GameObject stats in tankStats)
        {
            stats.SetActive(false);
        }
        swapButtons[0].SetActive(false);
        swapButtons[1].SetActive(false);
    }

    public void changeTank(int index)
    {
        foreach(GameObject stats in tankStats)
        {
            stats.SetActive(false);
        }
        tankStats[index].SetActive(true);
    }
    public void changeControls(string controls)
    {
        switch (controls)
        {
            case "Mouse & Keyboard":
                swapButtons[0].SetActive(false);
                swapButtons[1].SetActive(true);
                break;
            case "Gamepad":
                swapButtons[1].SetActive(false);
                swapButtons[0].SetActive(true);
                break;
        }
    }
}
