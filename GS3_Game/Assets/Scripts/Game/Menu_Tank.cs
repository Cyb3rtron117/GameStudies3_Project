using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static PlayerJoining;

public class Menu_Tank : MonoBehaviour
{
    public Animator anim;
    
    [Header("Tank Prefabs")]
    public List<GameObject> TankPrefabs = new List<GameObject>();
    private int tankIndex = 0;
    private GameObject currentTank;
    [SerializeField] private TankPrefab tankprefab;
    private List<Material> colours = new List<Material>();
    private int colourIndex = 0;   
    public Material activeMaterial;

    [SerializeField] private PlayerSetup playerSetup;

    [Header("Game Manager")]
    public GameObject gameManager;

    public void Setup(PlayerSetup setup)
    {
        playerSetup = setup;

        tankIndex = setup.tankIndex;
        colourIndex = setup.colourIndex;
    }


    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        colours = gameManager.GetComponent<Colours>().colours;
        ChangeTank(TankPrefabs[0]);
    }

    public void changeMaterial(Material newMat)
    {
        tankprefab.changeMaterial(newMat);        
    }

    //Changing Tanks
    public void ChangeTank(GameObject tank)
    {
        if (currentTank != null)
        {
            currentTank.SetActive(false);
        }

        currentTank = tank;
        currentTank.SetActive(true);

        tankprefab = currentTank.GetComponent<TankPrefab>();

        anim = tankprefab.animator;

        changeMaterial(activeMaterial);
    }
    private void NextTank()
    {
        tankIndex++;
        if(tankIndex >= TankPrefabs.Count)
        {
            tankIndex = 0;
        }
        playerSetup.tankIndex = tankIndex;
        ChangeTank(TankPrefabs[tankIndex]);
    }
    private void PreviousTank()
    {
        tankIndex--;
        if (tankIndex < 0)
        {
            tankIndex = TankPrefabs.Count - 1;
        }
        playerSetup.tankIndex = tankIndex;
        ChangeTank(TankPrefabs[tankIndex]);
    }
    public void ChangeTankInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        float direction = context.ReadValue<float>();

        if (direction > 0.5f)
        {
            NextTank();
        }
        else if (direction < -0.5f)
        {
            PreviousTank();
        }
    }

    //Changing Colours
    public void ChangeColour(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        float direction = context.ReadValue<float>();

        if (direction > 0.5f)
        {
            NextColour();
        }
        else if (direction < -0.5f)
        {
            PreviousColour();
        }
    }
    private void NextColour()
    {
        colourIndex++;

        if (colourIndex >= colours.Count)
        {
            colourIndex = 0;
        }
        playerSetup.colourIndex = colourIndex;
        ApplyColour();
    }
    private void PreviousColour()
    {
        colourIndex--;

        if (colourIndex < 0)
        {
            colourIndex = colours.Count - 1;
        }
        playerSetup.colourIndex = colourIndex;
        ApplyColour();
    }
    private void ApplyColour()
    {
        activeMaterial = colours[colourIndex];

        if (tankprefab != null)
        {
            tankprefab.changeMaterial(activeMaterial);
        }
    }
    public void PressReady(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }
        else
        {
            gameManager.GetComponent<ReadyUp>().playerReady(playerSetup.playerIndex);
        }
    }
}
