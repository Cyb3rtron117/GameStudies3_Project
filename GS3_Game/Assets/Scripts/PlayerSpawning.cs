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
        playerInput.transform.rotation = SpawnPoints[index].transform.rotation;
        playerInput.GetComponent<Menu_Tank>().activeMaterial = colours[index];
        /*CinemachineInputAxisController inputController = playerInput.GetComponentInChildren<CinemachineInputAxisController>();

        if (inputController != null)
        {
            inputController.PlayerIndex = index;
        }*/
    }
}
