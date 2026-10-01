using TMPro;
using UnityEngine;

public class Scoring : MonoBehaviour
{
    [SerializeField] private int Team1Score = 0;
    [SerializeField] private int Team2Score = 0;
    [SerializeField] private SceneLoading sceneScript;
    public TextMeshProUGUI team1Text;
    public TextMeshProUGUI team2Text;
    public GameObject[] T1Hearts;
    public GameObject[] T2Hearts;
    public void Score(Team _team)
    {
        switch(_team)
        {
            case Team.team1:
                Team1Score++;
                team1Text.SetText($"Team 1:\n{Team1Score}");
                team2Text.SetText($"Team 2:\n{Team2Score}");
                T2Hearts[Team1Score - 1].SetActive(false);
                if (Team1Score >= 3)
                {
                    sceneScript.loadMainMenu();
                }
                break;
            case Team.team2:
                Team2Score++;
                team1Text.SetText($"Team 1:\n{Team1Score}");
                team2Text.SetText($"Team 2:\n{Team2Score}");
                T1Hearts[Team2Score - 1].SetActive(false);
                if (Team2Score >= 3)
                {
                    sceneScript.loadMainMenu();
                }
                break;
            case Team.none:
                break;
        }
    }
    private void Start()
    {
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
}
