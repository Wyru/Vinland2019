using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneIn : MonoBehaviour
{
    public bool canStartLevel;
    
    private void OnEnable() {
        GetComponent<Animator>().SetTrigger("In");
    }
}
