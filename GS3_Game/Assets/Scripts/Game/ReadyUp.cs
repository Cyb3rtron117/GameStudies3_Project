using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ReadyUp : MonoBehaviour
{
    public List<bool> playersReady = new List<bool>();
    public GameObject[] CanvasReadyButtons;
    public SceneLoading _sceneLoading;
    [SerializeField] private int playersNeeded = 2;
    private void Start()
    {
        foreach(bool _bool in playersReady)
        {
            _bool.Equals(false);
        }
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
        }
    }
    public void playerReady(int which)
    {
        playersReady[which] = !playersReady[which];
        CanvasReadyButtons[which].SetActive(playersReady[which]);
        //print(playersReady[which]);
        checkReady();
    }
    private void checkReady()
    {
        if (playersReady.Count < playersNeeded)//will be 2 for actual game
        {
            print("Not enough players");
        }
        else
        {
            bool everyoneReady = true;
            for (int i = 0; i < playersReady.Count; i++)
            {
                if (!playersReady[i]) //if one is false
                {
                    everyoneReady = false;
                    break;
                }
            }
            if (everyoneReady)
            {
                print("Everyone ready");
                _sceneLoading.loadLevel();
            }
            else
            {
                print("Not everyone ready");
            }
        }
    }

}
