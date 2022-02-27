using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumUnit : Unit
{
    public MoveTileController attackPosition;

    [Header("Animations")]
    public float attackTime;
    public Animator hitboxAnimation;

    public override void UpdateUnit()
    {
    }

    public override void Attack()
    {
        // Placing hitbox
        attackHitbox.position = currentSelectedMove.transform.position;

        // Set correct orientation
        attackHitbox.localScale = new Vector2(attackHitbox.position.x > transform.position.x ? 1 : -1, attackHitbox.position.y > transform.position.y ? 1 : -1);

        // Play attack animation
        hitboxAnimation.SetTrigger("Attack");

        base.Attack();
    }

    public IEnumerator AttackRoutine()
    {

        yield return new WaitForSeconds(attackTime);

    }

    public override void AIMove()
    {

        currentSelectedMove = null;


        // Check for possible attack spots
        for (int i = 0; i < tiles.Length; i++)
        {
            if (CheckTileValid(tiles[i]) && !tiles[i].moveTile)
            {
                for (int k = 0; k < tiles[i].transform.childCount; k++)
                {
                    RaycastHit2D[] ray = Physics2D.RaycastAll(tiles[i].transform.GetChild(k).position, Vector2.zero);
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