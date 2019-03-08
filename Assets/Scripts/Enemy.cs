using UnityEngine;

public class Enemy : CharacterBase
{

    [Header("Behavior Parameters")]
    public bool moveWhileAttack;
    public bool chasePlayer;
    public float chaseStopDistance;

    public enum EnemyState
    {
        Chasing,
        Retreaten,
    };

    public EnemyState state;

    private Player player;

    override protected void Start()
    {
        base.Start();
    }

    override protected void Update()
    {
        base.Update();

        if (dead ||GameManager.Instance.gameOver)
            return;

        player = GameManager.Instance.player;

        if (chasePlayer)
        {
            if (Vector2.Distance(transform.position, player.transform.position) > chaseStopDistance)
            {
                if(!attacking && !attackCooldown){
                    if (transform.position.x > player.transform.position.x)
                    {
                        MoveLeft();
                    }
                    else
                    {
                        MoveRight();
                    }
                }
            }
            else
            {
                if(!attacking){
                    if (transform.position.x > player.transform.position.x)
                    {
                        MoveLeft();
                    }
                    else
                    {
                        MoveRight();
                    }
                }
                Idle();
                Attack();
            }
        }
    }
}