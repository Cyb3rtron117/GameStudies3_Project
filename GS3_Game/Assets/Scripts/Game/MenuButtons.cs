using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuButtons : MonoBehaviour
{
    public List<PlayerInput> PlayersJoined = new List<PlayerInput>();
    public Animator[] readyButtons;
    private void Start()
    {
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
                readyButtons[playercount].SetTrigger("HideKeyboard");
                break;
            case "Gamepad":
                readyButtons[playercount].SetTrigger("HideXbox");
                break;
        }
    }
}
