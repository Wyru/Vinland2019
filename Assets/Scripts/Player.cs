using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    [Header("Berserk")]
    public int maxBerserkValue;
    public int currentBerserk;
    [Range(0,1)]
    public float bloodToBerserk;
    public bool berserkActive;
    public float berserkDuration;
    public int berserkAttackMod;
    public int berserkLifeMod;
    public float lifeGainRate;
    public ParticleSystem berserkParticles;

    private float currentBerserkTime;
    private float currentLifeTime;
    private int normalAttack;

    private Material myMaterial;
    
    protected override void Start() {
        base.Start();
        OnDealDamage += GainBerserk;
    }

    protected override void Update()
    {
        base.Update();
        if (berserkActive)
        {
            if (currentBerserkTime < 0)
            {
                attack = normalAttack;
                berserkParticles.Clear();
                berserkParticles.Stop();
                berserkActive = false;
            }
            else
            {
                currentBerserkTime -= Time.deltaTime;

                if (currentLifeTime > lifeGainRate)
                {
                    currentLifeTime = 0;
                    GainLife(berserkLifeMod);
                }
                else
                {
                    currentLifeTime += Time.deltaTime;
                }

                currentBerserk = (int)(((float)maxBerserkValue*currentBerserkTime)/berserkDuration);

            }
        }
    }

    public void Awakening()
    {
        if(currentBerserk == maxBerserkValue && !berserkActive){
            berserkParticles.Play();
            animator.SetTrigger("Awakening");
            normalAttack = attack;
            attack += berserkAttackMod;
            currentBerserkTime = berserkDuration;
            currentLifeTime = 0;
            berserkActive = true;
        }
        
    }

    public override void TakeDamage(int value, Vector2 damageDir){
        base.TakeDamage(value, damageDir);
        CamBehavior.Instance.CamShake();
        GainBerserk(value);
    }

    public void GainBerserk(int value){
        currentBerserk += (int)(value*bloodToBerserk);
        if(currentBerserk > maxBerserkValue){
            currentBerserk = maxBerserkValue;
        }
    }

}
