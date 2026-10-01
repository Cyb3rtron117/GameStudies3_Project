using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoining : MonoBehaviour
{
    [SerializeField] public static List<PlayerSetup> playerSetups = new List<PlayerSetup>();

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
}

[System.Serializable]
public class PlayerSetup
{
    public int playerIndex;
    public int tankIndex;
    public InputDevice[] devices;

}


