using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(BoxCollider2D))]

public class TriggerChecker : MonoBehaviour
{
    public UnityEvent onTriggerEnter2D,onTriggerStay2D,onTriggerExit2D;
    
    private void OnTriggerEnter2D(Collider2D other) {
        onTriggerEnter2D.Invoke();
    }

    private void OnTriggerStay2D(Collider2D other) {
        onTriggerStay2D.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other) {
        onTriggerExit2D.Invoke();
    }

}
