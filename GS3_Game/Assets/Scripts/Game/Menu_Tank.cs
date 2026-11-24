using System.Collections;
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
    [SerializeField] private BotSetup botSetup;
    private int _playerIndex = 0;

    [Header("Game Manager")]
    public GameObject gameManager;
    private TeamColours _teamColours;
    public tankButtons tankButtonsScript;
    public HowPlay _howPlayScript;
    private SoundManager soundScript;
    private bool isBot = false;
    public void Setup(PlayerSetup setup)
    {
        isBot = false;
        playerSetup = setup;
        _playerIndex = playerSetup.playerIndex;
        tankIndex = setup.tankIndex;
        //colourIndex = setup.colourIndex;
    }
    public void BotSetup(BotSetup setup)
    {
        isBot = true;
        botSetup = setup;
        _playerIndex = botSetup.playerIndex;
        tankIndex = setup.tankIndex;
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        colours = gameManager.GetComponent<Colours>().colours;
        ChangeTank(tankIndex);
        _teamColours = gameManager.GetComponent<TeamColours>();
        readyAnim = GetComponent<Animator>();
        _howPlayScript = gameManager.GetComponent<HowPlay>();
        soundScript = gameManager.GetComponent<SoundManager>();
    }
    public void ChangeSlideInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        float direction = context.ReadValue<float>();

        _howPlayScript.ChangeSlideInput(direction);
    }
    public void OpenSlideInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        else
        {
            _howPlayScript.ShowSlides();
        }
    }

    public void changeMaterial(Material newMat)
    {
        tankprefab.changeMaterial(newMat);        
    }
    public void AssignButtonScript(tankButtons script)
    {
        tankButtonsScript = script;
        tankButtonsScript.changeTank(tankIndex);
    }

    //Changing Tanks
    public void ChangeTank(int tankIndex)
    {
        if (currentTank != null)
        {
            currentTank.SetActive(false);
        }

        if (playerSetup != null && !isBot)
        {
            playerSetup.tankIndex = tankIndex;
        }
        else if (botSetup != null && isBot)
        {
            botSetup.tankIndex = tankIndex;
        }

        currentTank = TankPrefabs[tankIndex];
        currentTank.SetActive(true);

        tankprefab = currentTank.GetComponent<TankPrefab>();

        anim = tankprefab.animator;
        shootVFX = tankprefab.shootVFX;
        changeMaterial(activeMaterial);
        tankButtonsScript.changeTank(tankIndex);
    }
    public void NextTank()
    {
        tankIndex++;
        if(tankIndex >= TankPrefabs.Count)
        {
            tankIndex = 0;
        }
        ChangeTank(tankIndex);
        if(soundScript != null)
        {
            soundScript.PlayClick();
        }
        
    }
    private void PreviousTank()
    {
        tankIndex--;
        if (tankIndex < 0)
        {
            tankIndex = TankPrefabs.Count - 1;
        }
        ChangeTank(tankIndex);
        if (soundScript != null)
        {
            soundScript.PlayClick();
        }
    }
    public void ChangeTankInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        if (isReady || HowPlay.TutisActive)
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
        if (playerSetup != null)
        {
            _teamColours.NextColour(playerSetup.playerIndex);
        }
        else if (botSetup != null)
        {
            _teamColours.NextColour(botSetup.playerIndex);
        }
        soundScript.PlayClick();

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

        if (playerSetup != null)
        {
            _teamColours.PreviousColour(playerSetup.playerIndex);
        }
        else if (botSetup != null)
        {
            _teamColours.PreviousColour(botSetup.playerIndex);
        }
        soundScript.PlayClick();

    }
    public void PressReady(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }
        else
        {
            ReadyAction();
        }
    }
    public void ReadyAction()
    {
        isReady = !isReady;
        readyAnim.SetBool("isReady", isReady);
        if (isReady)
        {
            shootVFX.Play();
        }
        gameManager.GetComponent<ReadyUp>().playerReady(_playerIndex);
        soundScript.PlayClick();

    }

    public void botReady()
    {
        ChangeTank(tankIndex);
        StartCoroutine(wait());
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        ReadyAction();
    }
}
