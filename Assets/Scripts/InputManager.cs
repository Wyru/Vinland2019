using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public static void GetPlayerInputs(Player player)
    {
        if(Input.GetKey(KeyCode.D)){
            player.MoveRight();
        }
        else if(Input.GetKey(KeyCode.A)){
            player.MoveLeft();
        }
        else{
            player.Idle();
        }
        
        if(Input.GetKeyDown(KeyCode.W)){
            player.Jump();
        }

        if(Input.GetKeyDown(KeyCode.P)){
            player.Attack();
        }

        if(Input.GetKeyDown(KeyCode.O)){
            player.Awakening();
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            player.Evade();
        }
    }
}
