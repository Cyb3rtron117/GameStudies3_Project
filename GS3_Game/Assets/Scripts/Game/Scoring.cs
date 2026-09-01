using UnityEngine;

public class Scoring : MonoBehaviour
{
    [SerializeField] private int Team1Score = 0;
    [SerializeField] private int Team2Score = 0;
    public void Score(Team _team)
    {
        switch(_team)
        {
            case Team.team1:
                Team1Score++;
                break;
            case Team.team2:
                Team2Score++;
                break;
            case Team.none:
                break;
        }
    }
}
