using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.Audio.GeneratorInstance;

public class PlayerSpawning : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public GameObject[] GoalTargets;
    public GameObject PlayerPrefab;
    public GameObject BotPrefab;
    private List <PlayerInput> _playerInputs = new List<PlayerInput>();
    private List<GameObject> Tanks = new List<GameObject>();
    public Material Team2Tank;
    public Material Team1Tank;

    [SerializeField] private List<GameObject> Team1 = new List<GameObject>();
    [SerializeField] private List<GameObject> Team2 = new List<GameObject>();
    private void Start()
    {
        _playerInputs.Clear();
        Tanks.Clear();
        InstantiatePlayers();
        InstantiateBots();
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
        player.GetComponent<Tank_Manager>().tankIndex = setup.tankIndex;
        player.GetComponent<Tank_Manager>().playerIndex = setup.playerIndex;
        player.GetComponent<Tank_Manager>().controlScheme = player.currentControlScheme;
        player.transform.position = SpawnPoints[index].transform.position;
        player.transform.rotation = SpawnPoints[index].transform.rotation;
        //On players 1 and 3, change tank material to team 1 and swap movement
        if (index % 2 == 0)
        {
            player.GetComponent<Tank_Manager>().activeMaterial = Team1Tank;
            player.GetComponent<Tank_Manager>().MoveMultiplier = -1;
        }
        else
        {
            player.GetComponent<Tank_Manager>().activeMaterial = Team2Tank;
            player.GetComponent<Tank_Manager>().MoveMultiplier = 1;
        }
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
            if(!Team1.Contains(player.gameObject))
            {
                Team1.Add(player.gameObject);
            }
        }
        else //Players 2 and 4
        {
            player.GetComponent<Tank_Manager>()._team = Team.team2;
            if (!Team2.Contains(player.gameObject))
            {
                Team2.Add(player.gameObject);
            }
        }
        FlipCamera(index, player);
    }
    private void FlipCamera(int index, PlayerInput player)
    {
        CinemachineOrbitalFollow CineOrbit = player.GetComponentInChildren<CinemachineOrbitalFollow>();
        if (CineOrbit != null)
        {
            //On players 1 and 3, flip the camera 180 degrees. Player 1 has index of 0, player 2 has 1, etc.
            if (index % 2 == 0)
            {
                CineOrbit.HorizontalAxis.Value = 180;
            }
            else
            {
                CineOrbit.HorizontalAxis.Value = 0;
            }
        }
        
    }
    private void InstantiateBots()
    {
        foreach (BotSetup setup in PlayerJoining.botSetups)
        {
            GameObject bot = GameObject.Instantiate(BotPrefab);
            if (!Tanks.Contains(bot))
            {
                Tanks.Add(bot);
            }
            SetupBot(bot, setup);
        }
    }
    private void SetupBot(GameObject bot, BotSetup setup)
    {
        int index = setup.playerIndex;
        bot.GetComponent<Bot_Tank_Manager>().tankIndex = setup.tankIndex;
        bot.GetComponent<Bot_Tank_Manager>().playerIndex = setup.playerIndex;
        bot.transform.position = SpawnPoints[index].transform.position;
        bot.transform.rotation = SpawnPoints[index].transform.rotation;
        //On players 1 and 3, change tank material to team 1 and swap movement
        if (index % 2 == 0)
        {
            bot.GetComponent<Bot_Tank_Manager>().activeMaterial = Team1Tank;
            bot.GetComponent<Bot_Tank_Manager>().MoveMultiplier = -1;
            bot.GetComponent<Bot_Tank_Manager>().goalTarget = GoalTargets[1];
        }
        else
        {
            bot.GetComponent<Bot_Tank_Manager>().activeMaterial = Team2Tank;
            bot.GetComponent<Bot_Tank_Manager>().MoveMultiplier = 1;
            bot.GetComponent<Bot_Tank_Manager>().goalTarget = GoalTargets[0];
        }
        
        //Assign players to teams
        if (index % 2 == 0) //Bots 1 and 3
        {
            bot.GetComponent<Bot_Tank_Manager>()._team = Team.team1;
            if (!Team1.Contains(bot.gameObject))
            {
                Team1.Add(bot.gameObject);
            }
        }
        else //Bots 2 and 4
        {
            bot.GetComponent<Bot_Tank_Manager>()._team = Team.team2;
            if (!Team2.Contains(bot.gameObject))
            {
                Team2.Add(bot.gameObject);
            }
        }
    }
    public void RespawnPlayers()
    {
        int rand1 = (UnityEngine.Random.Range(0, 2) * 2); //0 or 2
        int rand2 = (UnityEngine.Random.Range(0, 2) * 2) + 1; //1 or 3
        //Team 1
        for (int j = 0; j < Team1.Count; j++)
        {
            if(Team1[j].GetComponent<Tank_Manager>())
            {
                if(j == 0)
                {
                    Team1[j].GetComponent<Tank_Manager>().Die(rand1);
                }
                else
                {
                    if(rand1/2 == 1) //if 0, then returns 0. If 2, then returns 1
                    {
                        Team1[j].GetComponent<Tank_Manager>().Die(0);
                    }
                    else
                    {
                        Team1[j].GetComponent<Tank_Manager>().Die(2);
                    }
                }
            }
            else if (Team1[j].GetComponent<Bot_Tank_Manager>())
            {
                if (j == 0)
                {
                    Team1[j].GetComponent<Bot_Tank_Manager>().Die(rand1);
                }
                else
                {
                    if (rand1 / 2 == 1) //if 0, then returns 0. If 2, then returns 1
                    {
                        Team1[j].GetComponent<Bot_Tank_Manager>().Die(0);
                    }
                    else
                    {
                        Team1[j].GetComponent<Bot_Tank_Manager>().Die(2);
                    }
                }
            }
        }
        //Team 2
        for (int j = 0; j < Team1.Count; j++)
        {
            if (Team2[j].GetComponent<Tank_Manager>())
            {
                if (j == 0)
                {
                    Team2[j].GetComponent<Tank_Manager>().Die(rand2);
                }
                else
                {
                    if (rand2 % 3 == 0) //if 3, then returns 0. If 1, then returns 1
                    {
                        Team2[j].GetComponent<Tank_Manager>().Die(1);
                    }
                    else
                    {
                        Team2[j].GetComponent<Tank_Manager>().Die(3);
                    }
                }
            }
            else if (Team2[j].GetComponent<Bot_Tank_Manager>())
            {
                if (j == 0)
                {
                    Team2[j].GetComponent<Bot_Tank_Manager>().Die(rand2);
                }
                else
                {
                    if (rand2 % 3 == 0) //if 3, then returns 0. If 1, then returns 1
                    {
                        Team2[j].GetComponent<Bot_Tank_Manager>().Die(1);
                    }
                    else
                    {
                        Team2[j].GetComponent<Bot_Tank_Manager>().Die(3);
                    }
                }
            }
        }

        /*

        foreach (PlayerInput player in _playerInputs)
        {
            /*
            int index = player.playerIndex;
            player.transform.position = SpawnPoints[index].transform.position;
            player.transform.rotation = SpawnPoints[index].transform.rotation;
            FlipCamera(index, player);
            
            player.GetComponent<Tank_Manager>().Die();
            FlipCamera(player.playerIndex, player);
        }
        foreach (GameObject bot in Tanks)
        {
            bot.GetComponent<Bot_Tank_Manager>().Die();
        }*/
    }
    public static int Randomised(Team _team)
    {
        int rand = 0;
        switch (_team)
        {
            case Team.team1:
                rand = (UnityEngine.Random.Range(0, 1) * 2);
                break;

            case Team.team2:
                rand = (UnityEngine.Random.Range(0, 1) * 2) + 1;
                break;
        }
        return rand;
    }
}
