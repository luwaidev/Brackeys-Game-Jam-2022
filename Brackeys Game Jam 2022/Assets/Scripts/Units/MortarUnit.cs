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

    [Header("AI")]
    public float runDistance;

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

        currentSelectedMove = attackPosition;
        base.Attack();

        if (player == 1)
        {
            AIMove();
        }
    }

    public override void AIMove()
    {

        currentSelectedMove = null;

        // Find closest plaer unir
        Vector2 closestPlayerUnit = new Vector2(1000, 1000);
        Unit[] units = TurnManager.tm.units;
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] != null && units[i].player == 0)
            {
                closestPlayerUnit = (Vector2.Distance(transform.position, units[i].transform.position) <
                                     Vector2.Distance(transform.position, closestPlayerUnit)) ?
                                     (Vector2)units[i].transform.position : closestPlayerUnit;
            }
        }

        // if Player too close

        if (Vector2.Distance(transform.position, closestPlayerUnit) < runDistance)
        {
            MoveTileController furthest = tiles[0];
            for (int i = 0; i < tiles.Length; i++)
            {
                if (CheckTileValid(tiles[i]) && tiles[i].moveTile)
                {
                    furthest = (Vector2.Distance(transform.position, tiles[i].transform.position) <
                                Vector2.Distance(transform.position, furthest.transform.position)) ?
                                tiles[i] : furthest;
                }
            }

            currentSelectedMove = furthest;
            TurnManager.tm.LockMove();
            return;
        }
        else
        {
            // Check for possible attack spots
            for (int i = 0; i < tiles.Length; i++)
            {
                if (CheckTileValid(tiles[i]) && !tiles[i].moveTile)
                {

                    RaycastHit2D[] ray = Physics2D.RaycastAll(tiles[i].transform.position, Vector2.zero);
                    for (int j = 0; j < ray.Length; j++)
                    {
                        if (ray[j].collider.tag == "Unit" && ray[j].collider.GetComponent<Unit>().player != 1)
                        {
                            currentSelectedMove = tiles[i];
                            TurnManager.tm.LockMove();
                            return;
                        }
                    }
                }
            }

            // If can't find good attack spot move to closest
            MoveTileController closest = null;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (CheckTileValid(tiles[i]) && tiles[i].moveTile)
                {
                    if (closest == null) closest = tiles[i];
                    closest = (Vector2.Distance(transform.position, tiles[i].transform.position) >
                                Vector2.Distance(transform.position, closest.transform.position)) ?
                                tiles[i] : closest;

                }




            }
            currentSelectedMove = closest;
            TurnManager.tm.LockMove();
            return;
        }
    }
}
