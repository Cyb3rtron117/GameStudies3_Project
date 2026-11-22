using UnityEngine;
using UnityEngine.VFX;

public class ExplosionManager : MonoBehaviour
{
    public VisualEffect[] PlayerExplosions;
    public VisualEffect[] GoalExplosions;
    public void GoalExplode(Team _team)
    {
        switch (_team)
        {
            case Team.team1:
                GoalExplosions[1].Play(); //goal 2 explosion
                break;
            case Team.team2:
                GoalExplosions[0].Play(); //goal 1 explosion
                break;
            case Team.none:
                break;
        }
    }
    public void PlayerExplode(int which, Vector3 position)
    {
        Vector3 explodepos = new Vector3(position.x, 0, position.z);
        PlayerExplosions[which].gameObject.transform.position = explodepos;
        PlayerExplosions[which].Play();
    }
}
