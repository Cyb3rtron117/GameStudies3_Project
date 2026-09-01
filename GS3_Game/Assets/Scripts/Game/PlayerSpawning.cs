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
    private List <PlayerInput> _playerInputs = new List<PlayerInput>();

    private void Start()
    {
        _playerInputs.Clear();
        InstantiatePlayers();
    }

    private void InstantiatePlayers()
    {
        foreach(PlayerSetup setup in PlayerJoining.playerSetups)
        {
            PlayerInput player = PlayerInput.Instantiate(
                PlayerPrefab,
                playerIndex: setup.playerIndex,
                controlScheme: null,
                pairWithDevices: setup.devices
                );

            if(!_playerInputs.Contains(player))
            {
                _playerInputs.Add(player);
            }
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
        //Assign players to teams
        if (index % 2 == 0) //Players 1 and 3
        {
            player.GetComponent<Tank_Manager>()._team = Team.team1;
        }
        else //Players 2 and 4
        {
            player.GetComponent<Tank_Manager>()._team = Team.team2;
        }
        FlipCamera(index, player);
    }
    private void FlipCamera(int index, PlayerInput player)
    {
        //On players 1 and 3, flip the camera 180 degrees. Player 1 has index of 0, player 2 has 1, etc.
        if (index % 2 == 0)
        {
            CinemachineOrbitalFollow CineOrbit = player.GetComponentInChildren<CinemachineOrbitalFollow>();
            if (CineOrbit != null)
            {
                CineOrbit.HorizontalAxis.Value = 180;
            }
        }
    }
    public void RespawnPlayers()
    {
        foreach(PlayerInput player in _playerInputs)
        {
            int index = player.playerIndex;
            player.transform.position = SpawnPoints[index].transform.position;
            player.transform.rotation = SpawnPoints[index].transform.rotation;
            FlipCamera(index, player);
        }
    }
}
