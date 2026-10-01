using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using static PlayerJoining;

public class Menu_Tank : MonoBehaviour
{
    [Header("tank anim")]
    public Animator anim;
    [Header("ready up")]
    public Animator readyAnim;
    private bool isReady = false;
    
    [Header("Tank Prefabs")]
    public List<GameObject> TankPrefabs = new List<GameObject>();
    private int tankIndex = 0;
    private GameObject currentTank;
    [SerializeField] private TankPrefab tankprefab;
    private List<Material> colours = new List<Material>();
    private int colourIndex = 0;   
    public Material activeMaterial;
    public VisualEffect shootVFX;
    [SerializeField] private PlayerSetup playerSetup;

    [Header("Game Manager")]
    public GameObject gameManager;
    private TeamColours _teamColours;
    public void Setup(PlayerSetup setup)
    {
        playerSetup = setup;

        tankIndex = setup.tankIndex;
        //colourIndex = setup.colourIndex;
    }


    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        colours = gameManager.GetComponent<Colours>().colours;
        ChangeTank(TankPrefabs[0]);
        _teamColours = gameManager.GetComponent<TeamColours>();
        readyAnim = GetComponent<Animator>();
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
        shootVFX = tankprefab.shootVFX;
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
        if (isReady)
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
        if (isReady)
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
        /*
        colourIndex++;

        if (colourIndex >= colours.Count)
        {
            colourIndex = 0;
        }
        playerSetup.colourIndex = colourIndex;
        ApplyColour();*/

        _teamColours.NextColour(playerSetup.playerIndex);
    }
    private void PreviousColour()
    {
        /*
        colourIndex--;

        if (colourIndex < 0)
        {
            colourIndex = colours.Count - 1;
        }
        playerSetup.colourIndex = colourIndex;
        ApplyColour();*/

        _teamColours.PreviousColour(playerSetup.playerIndex);
    }
    private void ApplyColour()
    {
        //activeMaterial = colours[colourIndex];

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
            isReady = !isReady;
            readyAnim.SetBool("isReady", isReady);
            if(isReady)
            {
                shootVFX.Play();
            }
            gameManager.GetComponent<ReadyUp>().playerReady(playerSetup.playerIndex);
        }
    }
}
