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
    public float speed, jumpForce, evadeForce;

    [Header("Attack Parameters")]
    public bool attackWithProjectile;
    public float attackCooldownTime, attackRadius;
    public float projectileTrowForce;
    public GameObject projectile;
    public Transform attackAnchor;

    [Header("Other Configs")]
    public bool godMode;
    public bool takeDamageOnColide;
    public Transform groundCheckAnchor;
    public float groundCheckRadius;
    public float intangibleTime;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public string enemyTag;
    protected LayerMask myLayer;

    [Header("Blood")]
    public bool useBlood;
    public GameObject bloodParticle;
    public GameObject bloodSplash;

    [HideInInspector]
    public bool onJump, jumping, falling, moving, grounded, onEvading, evanding, dead, intangible;

    protected bool attackCooldown;
    protected bool attacking;
    protected bool onTakingDamage;
    protected bool takingDamage;
    protected bool onEndEvade;
    protected float currentAttackCooldown, currentIntangibleTime;
    protected Rigidbody2D rb;
    protected BoxCollider2D bc;
    protected Animator animator;
    protected SpriteMask sm;
    protected SpriteRenderer sr;

    protected Vector2 velocity;
    protected Vector2 damageImpulse;

    public delegate void DealDamage(int value);
    public static event DealDamage OnDealDamage;

    virtual protected void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        bc = this.GetComponent<BoxCollider2D>();
        animator = this.GetComponent<Animator>();
        sr = this.GetComponent<SpriteRenderer>();
        sm = this.GetComponent<SpriteMask>();
        myLayer = this.gameObject.layer;

        life = maxLife;
    }

    virtual protected void Update()
    {
        takingDamage = animator.GetCurrentAnimatorStateInfo(0).IsName("Damage");

        if (attackCooldown)
        {
            if (currentAttackCooldown > attackCooldownTime)
            {
                attackCooldown = false;
            }
            currentAttackCooldown += Time.deltaTime;
        }

        if (intangible)
        {
            if (currentIntangibleTime > intangibleTime)
            {
                intangible = false;
            }
            currentIntangibleTime += Time.deltaTime;
        }

        if (life <= 0)
        {
            if (!dead)
            {
                OnDead();
                dead = true;
            }
            if (useBlood)
            {
                sm.sprite = sr.sprite;
                sr.sortingOrder = -1;
            }
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

        if (!evanding && !intangible)
        {
            gameObject.layer = myLayer;
        }

        if (takingDamage)
        {
            if (onTakingDamage)
            {
                velocity = damageImpulse;
                intangible = true;
                StartCoroutine(FlashSprite());
                gameObject.layer = 13;
                currentIntangibleTime = 0;
                onTakingDamage = false;
            }
        }
        else if (evanding)
        {
            if (onEvading)
            {
                // on evade
                rb.AddForce(transform.right * evadeForce);
                gameObject.layer = 13;
                onEvading = false;
            }
            if (onEndEvade)
            {
                onEndEvade = false;
                evanding = false;
                velocity = Vector2.zero;
            }
        }
        else if (moving && !takingDamage)
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
        if (falling || onJump || jumping || takingDamage)
            return;

        onJump = true;
        animator.SetBool("Jumping", true);

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
        if (attackCooldown || evanding)
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
        Idle();
        currentAttackCooldown = 0;
    }

    // Need be added to animation event
    public void TrowProjectile()
    {
        GameObject go = Instantiate(projectile, attackAnchor.position, Quaternion.identity);
        go.GetComponent<ProjectileBehavior>().damage = attack;
        go.GetComponent<Rigidbody2D>().AddForce(transform.right * projectileTrowForce);
        Destroy(go, 5f);
    }

    // Need be added to animation event
    public virtual void DealAttackDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackAnchor.position, attackRadius, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<CharacterBase>().TakeDamage(attack, transform.right);
            if(OnDealDamage != null)
                OnDealDamage(attack);
        }
    }

    public virtual void Evade()
    {
        if (attacking || evanding || jumping || falling || takingDamage)
            return;

        animator.SetTrigger("Evade");
        evanding = true;
        onEvading = true;

    }

    public virtual void EndEvade()
    {
        onEndEvade = true;
    }

    public virtual void TakeDamage(int value)
    {
        TakeDamage(value, Vector2.zero);
    }

    public virtual void TakeDamage(int value, Vector2 damageDir)
    {
        if (evanding || takingDamage || intangible)
            return;

        animator.SetTrigger("Damage");

        damageImpulse = damageDir.normalized * value;
        onTakingDamage = true;
        if (!godMode)
            life -= value;

        if (useBlood && bloodParticle != null)
        {
            GameObject go = Instantiate(bloodParticle, this.transform.position, Quaternion.identity);
            Destroy(go, 2f);
        }

        attacking = false;
    }

    public void EndAttack()
    {
        attacking = false;
    }

    virtual protected void OnDead()
    {
        animator.SetTrigger("Die");
        if (useBlood && bloodSplash != null)
        {
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

    virtual protected void OnCollisionEnter2D(Collision2D other)
    {
        if (takeDamageOnColide && other.gameObject.CompareTag(enemyTag))
        {
            Vector2 v = rb.velocity != Vector2.zero ? -rb.velocity : other.gameObject.GetComponent<Rigidbody2D>().velocity;
            TakeDamage(other.gameObject.GetComponent<CharacterBase>().attack, v);
        }
    }

    protected virtual void GainLife(int value){
        life+=value;
        if(life > maxLife)
            life = maxLife;
    }

    IEnumerator FlashSprite()
    {
        if (intangibleTime > 0)
        {
            while (intangible)
            {
                if (sr.material.GetFloat("_FlashAmount") > 0)
                {
                    sr.material.SetFloat("_FlashAmount", 0);
                }
                else
                {
                    sr.material.SetFloat("_FlashAmount", .8f);
                }
                yield return new WaitForSeconds(.1f);
            }
            sr.material.SetFloat("_FlashAmount", 0);
        }

    }


}