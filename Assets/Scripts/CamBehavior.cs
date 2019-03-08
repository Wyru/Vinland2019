using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBehavior : MonoBehaviour
{
    public static CamBehavior Instance;
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        ani = this.GetComponent<Animator>();
        Instance = this;
    }

    public void CamShake(){
        ani.SetTrigger("Shake");
    }
}
