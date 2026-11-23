using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoining : MonoBehaviour
{
    [SerializeField] public static List<PlayerSetup> playerSetups = new List<PlayerSetup>();
    [SerializeField] public static List<BotSetup> botSetups = new List<BotSetup>();
    public GameObject menuTankPrefab;

    public Transform[] SpawnPoints;
    public TeamColours teamColoursScript;
    private MenuButtons _menuButtonsScript;
    private int _coloursShown = 0;
    public PlayerInputManager _playerInputManager;
    private void Start()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        _coloursShown = 0;
        playerSetups.Clear();
        botSetups.Clear();
        _menuButtonsScript = GetComponent<MenuButtons>();
        SplitscreenDivider.PlayerAmt = 0;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;
        playerInput.transform.position = SpawnPoints[index].transform.position;
        playerInput.transform.rotation = SpawnPoints[index].transform.rotation;
        print(playerInput.currentControlScheme);
        //On players 1 and 3
        if (index % 2 == 0)
        {
            playerInput.GetComponent<Menu_Tank>().activeMaterial = GetComponent<TeamColours>().Team1Tank;
        }
        else
        {
            playerInput.GetComponent<Menu_Tank>().activeMaterial = GetComponent<TeamColours>().Team2Tank;
        }
        PlayerSetup setup = new PlayerSetup
        {
            playerIndex = playerInput.playerIndex,
            devices = playerInput.devices.ToArray(),
            tankIndex = 0
        };
        playerSetups.Add(setup);
        _menuButtonsScript.AddPlayer(playerInput);
        Menu_Tank menuTank = playerInput.GetComponent<Menu_Tank>();

        if (menuTank != null)
        {
            menuTank.Setup(setup);
            menuTank.AssignButtonScript(_menuButtonsScript.tankButtonScripts[index]);
            _menuButtonsScript.tankButtonScripts[index].changeControls(playerInput.currentControlScheme);
        }
        GetComponent<ReadyUp>().addPlayer();
        if(_coloursShown < 2)
        {
            _coloursShown++;
            _menuButtonsScript.ShowColourSwitch(_coloursShown);
        }
        _menuButtonsScript.ShowReadyButton(index);
        SplitscreenDivider.PlayerAmt++;

        if(index == 0)
        {
            GetComponent<HowPlay>().ShowPrompt();
        }
    }
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;
        Destroy(playerInput.gameObject);
    }

    public void FillBots()
    {
        float BotRerollDelay = 0;
        _playerInputManager.DisableJoining();
        for (int i = playerSetups.Count; i < 4; i++)
        {
            print("adding bot");
            int index = i;
            GameObject bot = GameObject.Instantiate(menuTankPrefab);
            bot.transform.position = SpawnPoints[index].transform.position;
            bot.transform.rotation = SpawnPoints[index].transform.rotation;
            //On players 1 and 3
            if (index % 2 == 0)
            {
                bot.GetComponent<Menu_Tank>().activeMaterial = GetComponent<TeamColours>().Team1Tank;
            }
            else
            {
                bot.GetComponent<Menu_Tank>().activeMaterial = GetComponent<TeamColours>().Team2Tank;
            }
            BotSetup setup = new BotSetup
            {
                playerIndex = index,
                tankIndex = 0
            };
            botSetups.Add(setup);
            _menuButtonsScript.AddBot(index);
            Menu_Tank menuTank = bot.GetComponent<Menu_Tank>();
            
            if (menuTank != null)
            {
                menuTank.BotSetup(setup);
                menuTank.AssignButtonScript(_menuButtonsScript.tankButtonScripts[index]);
            }
            _menuButtonsScript.ShowReadyButton(index);
            GetComponent<ReadyUp>().playersReady.Add(false);
            bot.GetComponent<BotReroll>().StartLoop(BotRerollDelay);
            BotRerollDelay += 0.5f;
        }
    }
}

[Serializable]
public class PlayerSetup
{
    public int playerIndex;
    public int tankIndex;
    public InputDevice[] devices;

}
[Serializable]
public class BotSetup
{
    public int playerIndex;
    public int tankIndex;
}


