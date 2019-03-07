using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public string playerTag;
    public string groundTag;

    private bool alreadyDealDamage;

    [HideInInspector]
    public int damage;

    public void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag(playerTag) && !alreadyDealDamage){
            other.gameObject.GetComponent<Player>().TakeDamage(damage);
            alreadyDealDamage = false;
        }
        if(other.gameObject.CompareTag(groundTag)){
            Destroy(this);
        }
    }

    private void OnDestroy() {
        Destroy(this.GetComponent<Rigidbody2D>());
        Destroy(this.GetComponent<BoxCollider2D>());
    }
}
