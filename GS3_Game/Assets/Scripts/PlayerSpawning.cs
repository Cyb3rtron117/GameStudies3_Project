using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerSpawning : MonoBehaviour
{
    public Transform[] SpawnPoints;
    [SerializeField] private List<Material> colours = new List<Material>();
    [SerializeField] private int playerCount = 0;
    private void Start()
    {
        colours = GetComponent<Colours>().colours;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.position = SpawnPoints[playerCount].transform.position;
        playerInput.GetComponent<Tank_Manager>().activeMaterial = colours[playerCount];
        playerCount++;
    }
}
