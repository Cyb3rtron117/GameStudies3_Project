using TMPro;
using UnityEngine;

public class Scoring : MonoBehaviour
{
    [SerializeField] private int Team1Score = 0;
    [SerializeField] private int Team2Score = 0;
    public TextMeshProUGUI team1Text;
    public TextMeshProUGUI team2Text;
    public void Score(Team _team)
    {
        switch(_team)
        {
            case Team.team1:
                Team1Score++;
                team1Text.SetText($"Team 1:\n{Team1Score}");
                team2Text.SetText($"Team 2:\n{Team2Score}");
                break;
            case Team.team2:
                Team2Score++;
                team1Text.SetText($"Team 1:\n{Team1Score}");
                team2Text.SetText($"Team 2:\n{Team2Score}");
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
    }
}
