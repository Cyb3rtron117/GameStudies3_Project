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
    private void Start()
    {
        playerSetups.Clear();
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;
        playerInput.transform.position = SpawnPoints[index].transform.position;
        playerInput.transform.rotation = SpawnPoints[index].transform.rotation;
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
        Menu_Tank menuTank = playerInput.GetComponent<Menu_Tank>();

        if (menuTank != null)
        {
            menuTank.Setup(setup);
        }
        GetComponent<ReadyUp>().playersReady.Add(false);
    }
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;
        Destroy(playerInput.gameObject);
    }

    public void FillBots()
    {
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
                tankIndex = UnityEngine.Random.Range(0, 4) //randomly select between the 4 tanks
            };
            botSetups.Add(setup);

            Menu_Tank menuTank = bot.GetComponent<Menu_Tank>();

            if (menuTank != null)
            {
                menuTank.BotSetup(setup);
            }
            
            GetComponent<ReadyUp>().playersReady.Add(false);
            bot.GetComponent<Menu_Tank>().botReady();
        }
    }
}

[System.Serializable]
public class PlayerSetup
{
    public int playerIndex;
    public int tankIndex;
    public InputDevice[] devices;

}
[System.Serializable]
public class BotSetup
{
    public int playerIndex;
    public int tankIndex;
}


