using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarUnit : Unit
{
    public bool charging;
    public int attackTimer;
    public int attackChargeTime;
    public MoveTileController attackPosition;

    [Header("Animations")]
    public float attackTime;
    public Animator chargeAnim;
    public Animator hitboxAnimation;

    public override void UpdateUnit()
    {
        if (charging)
        {
            attackTimer++;
        }

        if (attackTimer >= attackChargeTime)
        {
            attackTimer = 0;
            attack = true;
            charging = false;
        }
    }

    public override void Attack()
    {
        if (attack)
        {
            chargeAnim.SetBool("Charging", false);
            StartCoroutine(AttackRoutine());
            attack = false;
        }
        else
        {
            charging = true;
            chargeAnim.SetBool("Charging", true);
            attackPosition = currentSelectedMove;
        }
    }

    public IEnumerator AttackRoutine()
    {
        currentSelectedMove = attackPosition;

        // Placing hitbox
        attackHitbox.position = currentSelectedMove.transform.position;

        // Play attack animation
        hitboxAnimation.SetTrigger("Attack");
        yield return new WaitForSeconds(attackTime);

        base.Attack();
    }
}
