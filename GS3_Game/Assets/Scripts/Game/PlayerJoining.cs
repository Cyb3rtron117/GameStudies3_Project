using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoining : MonoBehaviour
{
    [SerializeField] public static List<PlayerSetup> playerSetups = new List<PlayerSetup>();

    public Transform[] SpawnPoints;
    private void Start()
    {
        
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;
        playerInput.transform.position = SpawnPoints[index].transform.position;
        playerInput.transform.rotation = SpawnPoints[index].transform.rotation;
        playerInput.GetComponent<Menu_Tank>().activeMaterial = GetComponent<Colours>().colours[index];
        PlayerSetup setup = new PlayerSetup
        {
            playerIndex = playerInput.playerIndex,
            devices = playerInput.devices.ToArray(),
            tankIndex = 0,
            colourIndex = playerInput.playerIndex
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
    public int colourIndex;
    public InputDevice[] devices;

}


