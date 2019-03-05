using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Character Parameters")]
    public int life;
    public int maxLife;
    public int attack;
    public float speed, jumpForce, evadeForce, iFTime;

    [Header("Attack Parameters")]
    public bool attackWithProjectile;
    public float attackCooldownTime, attackRadius;
    public float projectileTrowForce;
    public GameObject projectile;
    public Transform attackAnchor;

    [Header("Other Configs")]
    public bool godMode;
    public Transform groundCheckAnchor;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

    [Header("Blood")]
    public bool useBlood;
    public GameObject bloodParticle;
    public GameObject bloodSplash;

    [HideInInspector]
    public bool onJump, jumping, falling, moving, grounded, onEvanding, evanding, dead;

    protected bool attackCooldown;
    protected bool attacking;
    protected float currentAttackCooldown, currentIF;
    protected Rigidbody2D rb;
    protected BoxCollider2D bc;
    protected Animator animator;
    protected SpriteMask sm;
    protected SpriteRenderer sr;

    Vector2 velocity;


    virtual protected void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        bc = this.GetComponent<BoxCollider2D>();
        animator = this.GetComponent<Animator>();
        sr = this.GetComponent<SpriteRenderer>();
        sm = this.GetComponent<SpriteMask>();

        life = maxLife;
    }

    virtual protected void Update()
    {

        if (evanding)
        {
            if (currentIF > iFTime)
            {
                evanding = false;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                bc.enabled = true;
            }
            currentIF += Time.deltaTime;
        }

        if (attackCooldown)
        {
            if (currentAttackCooldown > attackCooldownTime)
            {
                attackCooldown = false;
            }
            currentAttackCooldown += Time.deltaTime;
        }

        if (life <= 0)
        {
            if (!dead)
            {
                OnDead();
                dead = true;
            }
            sm.sprite = sr.sprite;
            sr.sortingOrder = -1;
        }
    }

    private void FixedUpdate()
    {
        if (dead)
            return;

        CheckGrounded();

        velocity = this.rb.velocity;

        if (velocity.y < -1)
        {
            falling = true;
            jumping = false;
        }

        if (onJump)
        {
            this.rb.AddForce(transform.up * jumpForce);
            onJump = false;
            jumping = true;
        }
        else if (jumping)
        {

        }
        else if (falling)
        {
            // velocity.y = (-transform.up * fallSpeed).y;
        }

        if (evanding)
        {
            if (onEvanding)
            {
                // on evade
                rb.AddForce(transform.right * evadeForce);
                rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                bc.enabled = false;
                onEvanding = false;
            }
        }
        else if (moving)
        {
            velocity.x = (transform.right * speed).x;
        }
        else
        {
            velocity.x = 0;
        }

        this.rb.velocity = velocity;
    }

    public void MoveLeft()
    {
        if (attacking)
            return;

        moving = true;
        transform.eulerAngles = new Vector3(0, 180, 0);
        animator.SetBool("Moving", true);
    }

    public void MoveRight()
    {
        if (attacking)
            return;

        moving = true;
        transform.eulerAngles = new Vector3(0, 0, 0);
        animator.SetBool("Moving", true);
    }

    public void Jump()
    {
        if (falling || onJump || jumping)
            return;

        onJump = true;
        animator.SetBool("Jumping", true);
        Debug.Log("Jump");

    }


    public void Idle()
    {
        moving = false;
        animator.SetBool("Moving", false);
    }

    protected void CheckGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheckAnchor.position, groundCheckRadius, groundLayer);
        if (hit != null)
        {
            grounded = true;
            if (falling)
            {
                falling = false;
                animator.SetBool("Jumping", false);
            }

        }
        else
        {
            grounded = false;
            if (!jumping)
            {
                falling = true;
            }
        }
    }


    virtual protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheckAnchor.position, groundCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackAnchor.position, attackRadius);
    }

    public virtual void Attack()
    {
        if (attackCooldown || moving || evanding)
            return;

        if (falling || jumping)
        {
            rb.velocity.Set(0, 0);
            animator.SetTrigger("Attack02");
        }
        else
        {
            animator.SetTrigger("Attack01");
        }

        attackCooldown = true;
        attacking = true;
        currentAttackCooldown = 0;
        Debug.Log("Attack");
    }

    // Need be added to animation event
    public void TrowProjectile()
    {
        GameObject go = Instantiate(projectile, attackAnchor.position, Quaternion.identity);
        go.GetComponent<ProjectileBehavior>().damage = attack;
        go.GetComponent<Rigidbody2D>().AddForce(transform.right * projectileTrowForce);
        Destroy(go,5f);
    }

    // Need be added to animation event
    public virtual void DealAttackDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackAnchor.position, attackRadius, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            Debug.Log(hit.gameObject.name);
            hit.GetComponent<CharacterBase>().TakeDamage(attack);
        }
    }

    public virtual void Evade()
    {
        if (attacking || evanding || jumping || falling)
            return;

        animator.SetTrigger("Evade");
        currentIF = 0;
        evanding = true;
        onEvanding = true;

    }

    public virtual void TakeDamage(int value)
    {
        if (evanding)
            return;

        animator.SetTrigger("Damage");
        if (!godMode)
            life -= value;
        if(useBlood && bloodParticle != null){
            GameObject go = Instantiate(bloodParticle, this.transform.position, Quaternion.identity);
            Destroy(go,2f);
        }
        attacking = false;
    }

    public void EndAttack()
    {
        attacking = false;
    }

    virtual protected void OnDead()
    {
        Debug.Log("On Dead");
        animator.SetTrigger("Die");
        if(useBlood && bloodSplash != null){
            Instantiate(bloodSplash, this.transform);
        }
        rb.simulated = false;
        bc.enabled = false;
    }

    private void OnDestroy()
    {
        Destroy(this.GetComponent<Rigidbody2D>());
        Destroy(this.GetComponent<BoxCollider2D>());
    }

}