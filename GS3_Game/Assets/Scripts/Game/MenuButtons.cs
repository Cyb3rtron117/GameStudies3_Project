using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public List<PlayerInput> PlayersJoined = new List<PlayerInput>();
    public Animator[] joinButtons;
    public tankButtons[] tankButtonScripts;
    public RawImage team1Image;
    public RawImage team2Image;
    public GameObject[] team1ColourButtons;
    public GameObject[] team2ColourButtons;

    [Header("Ready Up")]
    public GameObject[] readyCanvas;
    public Image[] readyButtons;
    public Color readycolour;
    public Color unReadycolour;
    private void Start()
    {
        foreach (GameObject obj in team1ColourButtons)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in team2ColourButtons)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in readyCanvas)
        {
            obj.SetActive(false);
        }
        PlayersJoined.Clear();
    }
    public void AddPlayer(PlayerInput player)
    {
        PlayersJoined.Add(player);
        int playercount = PlayersJoined.Count - 1;

        string controlScheme = player.currentControlScheme;
        switch (controlScheme)
        {
            case "Mouse & Keyboard":
                joinButtons[playercount].SetTrigger("HideKeyboard");
                break;
            case "Gamepad":
                joinButtons[playercount].SetTrigger("HideXbox");
                break;
        }
    }
    public void ShowReadyButton(int which)
    {
        readyCanvas[which].SetActive(true);
    }
    public void ToggleReadyButton(int which, bool active)
    {
        if(active)
        {
            readyButtons[which].color = readycolour;
        }
        else
        {
            readyButtons[which].color = unReadycolour;
        }
    }

    public void ShowColourSwitch(int which)
    {
        switch (which)
        {
            case 1:
                foreach (GameObject obj in team1ColourButtons)
                {
                    obj.SetActive(true);
                }
                break;
            case 2:
                foreach (GameObject obj in team2ColourButtons)
                {
                    obj.SetActive(true);
                }
                break;
        }
    }
}
