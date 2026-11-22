using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class ReadyUp : MonoBehaviour
{
    public List<bool> playersReady = new List<bool>();
    public GameObject[] CanvasReadyButtons;
    public SceneLoading _sceneLoading;
    [SerializeField] private int playersNeeded = 2;
    [SerializeField] private PlayerJoining _playerJoiningScript;
    [SerializeField] private float MaxPlayerWaitTime;
    [SerializeField] private float CurrentPlayerWaitTime;
    public bool startPlayerWait = false;
    [SerializeField] private Animator ReadyAnim;
    [SerializeField] private TextMeshProUGUI playerWaitText;
    private void Start()
    {
        resetTimer();
        startPlayerWait = false;
        foreach (bool _bool in playersReady)
        {
            _bool.Equals(false);
        }/*
        if (CanvasReadyButtons.Length == 0)
        {
            Debug.LogWarning("No Buttons assigned to playerjoin script!");
        }
        else
        {
            foreach (GameObject obj in CanvasReadyButtons)
            {
                obj.SetActive(false);
            }
        }*/
    }
    public void addPlayer()
    {
        playersReady.Add(false);
        startPlayerWait = false;
    }
    public void playerReady(int which)
    {
        playersReady[which] = !playersReady[which];
        GetComponent<MenuButtons>().ToggleReadyButton(which, playersReady[which]);
        //CanvasReadyButtons[which].SetActive(playersReady[which]);
        //print(playersReady[which]);
        resetTimer();
        startPlayerWait = false;
        checkReady();
    }
    private void checkReady()
    {
        if (playersReady.Count < 4) //if theres not 4 players, both people and bots
        {
            if (playersReady.Count < PlayerJoining.playerSetups.Count)//tests against how many players joined
            {
                print("Not enough players");
            }
            else
            {
                bool peopleReady = true;
                for (int i = 0; i < playersReady.Count; i++)
                {
                    if (!playersReady[i]) //if one is false
                    {
                        peopleReady = false;
                        break;
                    }
                }
                if (peopleReady)
                {
                    print("All human players ready");
                    resetTimer();
                    startPlayerWait = true;
                }
                else
                {
                    print("Not everyone ready");
                }
            }
        }
        else //if theres 4 players
        {
            bool allReady = true;
            for (int i = 0; i < playersReady.Count; i++)
            {
                if (!playersReady[i]) //if one is false
                {
                    allReady = false;
                    break;
                }
            }
            if (allReady)
            {
                print("Everyone ready");
                StartCoroutine(Startgame());
            }
            else
            {
                print("Not everyone ready");
            }
        }
    }
    private IEnumerator Startgame()
    {
        yield return new WaitForSeconds(1f);
        _sceneLoading.loadLevel();
    }
    private void resetTimer()
    {
        CurrentPlayerWaitTime = MaxPlayerWaitTime + 0.5f;
    }
    private void FixedUpdate()
    {
        ReadyAnim.SetBool("Show",startPlayerWait);
        if (startPlayerWait)
        {
            if(CurrentPlayerWaitTime > 0)
            {
                CurrentPlayerWaitTime -= Time.fixedDeltaTime;
                playerWaitText.SetText($"Waiting for players: {CurrentPlayerWaitTime:F0}s");
            }
            else
            {
                playerWaitText.SetText("Waiting for players: 0s");
                startPlayerWait = false;
                _playerJoiningScript.FillBots();
            }
        }
    }
}
