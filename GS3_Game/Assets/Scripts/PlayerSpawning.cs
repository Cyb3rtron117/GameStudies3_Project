using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerSpawning : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public GameObject PlayerPrefab;

    private void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        foreach(PlayerSetup setup in PlayerJoining.playerSetups)
        {
            PlayerInput player = PlayerInput.Instantiate(
                PlayerPrefab,
                playerIndex: setup.playerIndex,
                controlScheme: null,
                pairWithDevices: setup.devices
                );

            SetupPlayer(player, setup);
        }
    }
    private void SetupPlayer(PlayerInput player, PlayerSetup setup)
    {
        int index = player.playerIndex;
        player.transform.position = SpawnPoints[index].transform.position;
        player.transform.rotation = SpawnPoints[index].transform.rotation;
        player.GetComponent<Tank_Manager>().colourIndex = setup.colourIndex;
        player.GetComponent<Tank_Manager>().tankIndex = setup.tankIndex;

        //Cinemachine
        CinemachineInputAxisController inputController = player.GetComponentInChildren<CinemachineInputAxisController>();

        if (inputController != null)
        {
            inputController.PlayerIndex = index;
        }

        //On players 1 and 3, flip the camera 180 degrees. Player 1 has index of 0, player 2 has 1, etc.
        if (index % 2 == 0)
        {
            CinemachineOrbitalFollow CineOrbit = player.GetComponentInChildren<CinemachineOrbitalFollow>();
            if(CineOrbit != null)
            {
                CineOrbit.HorizontalAxis.Value = 180;
            }
        }
    }
}
