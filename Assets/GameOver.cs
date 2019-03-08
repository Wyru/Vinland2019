using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;

    private void OnEnable() {
        
        GetComponent<Animator>().SetTrigger("In");
    }

    public void Out() {
        GetComponent<Animator>().SetTrigger("Out");
    }

}


