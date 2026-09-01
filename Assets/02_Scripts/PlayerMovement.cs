using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform Player;

    private void Start()
    {
        if (Player == null)
        {
            Player = this.transform;
        }
    }

    public bool Walk(float Speed)
    {
        Player.position = Vector3.MoveTowards(Player.transform.position, Player.transform.position + Player.transform.forward, Speed * Time.deltaTime);

        return true;
    }

    public float TargetWalk(float target, float Speed, bool isx)
    {

        Vector3 Target = Player.transform.position;
        if (isx)
        {
            Target.x = target;
        }
        else
        {
            Target.z = target;
        }

        
        if(Vector3.Distance(Player.position, Target) < 0.1f)
        {
            Player.position = Target;
            return 0f;
        }
        else
        {
            Player.position = Vector3.MoveTowards(Player.transform.position, Target, Speed * Time.deltaTime);
        }
        return Vector3.Distance(Player.position, Target);
    }
}
