using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{
    [SerializeField] private int Team1Score = 0;
    [SerializeField] private int Team2Score = 0;
    [SerializeField] private SceneLoading sceneScript;
    [SerializeField] private ExplosionManager explodeScript;
    [SerializeField] private PlayerSpawning spawnScript;
    public TextMeshProUGUI team1Text;
    public TextMeshProUGUI team2Text;
    public GameObject[] T1Hearts;
    public GameObject[] T2Hearts;
    private WaitForSeconds EndDelay = new WaitForSeconds(3f);
    public Animator winAnim;
    public TextMeshProUGUI Wintext;
    public static bool GameEnd = false;
    [SerializeField] private Material[] teamColours;
    [SerializeField] private RawImage winColour;
    public void Score(Team _team)
    {
        switch(_team)
        {
            case Team.team1:
                Team1Score++;
                team1Text.SetText($"Team 1:\n{Team1Score}");
                team2Text.SetText($"Team 2:\n{Team2Score}");
                if (Team1Score <= 2)
                {
                    T2Hearts[Team1Score - 1].SetActive(false);
                    spawnScript.RespawnPlayers();
                }
                else
                {
                    T2Hearts[2].SetActive(false);
                    EndGame(0);
                }

                break;

            case Team.team2:
                Team2Score++;
                team1Text.SetText($"Team 1:\n{Team1Score}");
                team2Text.SetText($"Team 2:\n{Team2Score}");
                if(Team2Score <= 2)
                {
                    T1Hearts[Team2Score - 1].SetActive(false);
                    spawnScript.RespawnPlayers();
                }
                else
                {
                    T1Hearts[2].SetActive(false);
                    EndGame(1);
                }

                break;
            case Team.none:
                break;
        }
        explodeScript.GoalExplode(_team);
    }
    private void Start()
    {
        spawnScript = GetComponent<PlayerSpawning>();
        GameEnd = false;
        print($"Game end set to {GameEnd}");
        Team1Score = 0;
        Team2Score = 0;
        team1Text.SetText($"Team 1:\n{Team1Score}");
        team2Text.SetText($"Team 2:\n{Team2Score}");
        foreach(GameObject heart in T1Hearts)
        {
            heart.SetActive(true);
        }
        foreach (GameObject heart in T2Hearts)
        {
            heart.SetActive(true);
        }
    }

    public void EndGame(int which)
    {
        Texture2D texture = teamColours[0].GetTexture("_Team_Texture") as Texture2D;
        if(which == 1)
        {
            texture = teamColours[1].GetTexture("_Team_Texture") as Texture2D;
        }
        // Second row from top, first column
        Color sampledColor = texture.GetPixel(8, 39);
        
        Wintext.color = sampledColor;

        sampledColor.a = winColour.color.a; //overwrites the alpha value, after the text has been recoloured
        winColour.color = sampledColor;

        GameEnd = true;
        print($"Game end set to {GameEnd}");
        StartCoroutine(ChangeScenes());
        winAnim.SetTrigger("Show");
        Wintext.SetText($"Team {which + 1}");
    }
    IEnumerator ChangeScenes()
    {
        yield return EndDelay;
        sceneScript.loadMainMenu();
    }
}
