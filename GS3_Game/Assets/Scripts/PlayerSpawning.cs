using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.UIElements;

public class PlayerSpawning : MonoBehaviour
{
    public Transform[] SpawnPoints;
    [SerializeField] private List<Material> colours = new List<Material>();
    private void Start()
    {
        colours = GetComponent<Colours>().colours;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;
        playerInput.transform.position = SpawnPoints[index].transform.position;
        playerInput.GetComponent<Tank_Manager>().activeMaterial = colours[index];
        CinemachineInputAxisController inputController = playerInput.GetComponentInChildren<CinemachineInputAxisController>();

        if (inputController != null)
        {
            inputController.PlayerIndex = index;
        }
    }
}
