using UnityEngine;

public class PlayerStates : MonoBehaviour
{

    public enum States{ isIdle, isWalking, isJumping }

    public States currentState;
    
    void Update()
    {
        /* if {jump}
         currentState = States.isJumping;
         */
        switch (currentState)
        {
            case States.isIdle:
                HandleIdle();
                break;
            case States.isWalking:
                HandleWalking();
                break;
            case States.isJumping:
                HandleJumping();
                break;
        }
    }

    void HandleIdle()
    {
        //play idle anims

        /* if {walking}
         currentState = States.isWalking;
         */
    }
    void HandleWalking()
    {
        //play walking anims

        /* if {not walking}
         currentState = States.isIdle;
         */
    }
    void HandleJumping()
    {
        //play jump anims

        /* if {not jumping}
         currentState = States.isIdle;
         */
    }
}
