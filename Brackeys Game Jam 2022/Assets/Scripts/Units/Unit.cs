using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    bool selected;
    public MoveTileController[] tiles;
    public MoveTileController currentSelectedMove;
    public int player;

    [Header("Combat Settings")]

    public Transform attackHitbox;
    public int attackDamage;
    public int health;

    public virtual void SetActive()
    {
        currentSelectedMove = null;
        selected = true;
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].gameObject.SetActive(tiles[i].moveTile && CheckTileValid(tiles[i]));
        }

        if (player == 1)
        {
            AIMove();
        }
    }

    public virtual void SetAsleep()
    {
        currentSelectedMove = null;
        selected = false;
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].gameObject.SetActive(false);
        }
    }


    public virtual void LockMove()
    {
        if (currentSelectedMove == null) return;
        if (currentSelectedMove.moveTile)
        {
            Move();
        }
        else
        {
            Attack();
        }
    }

    public virtual void SetMode(bool moving)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].gameObject.SetActive(tiles[i].moveTile == moving && CheckTileValid(tiles[i]));
        }
    }

    public virtual void OnHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Private
    public virtual bool CheckTileValid(MoveTileController tile)
    {
        RaycastHit2D[] ray = Physics2D.RaycastAll(tile.transform.position, Vector2.zero);

        for (int i = 0; i < ray.Length; i++)
        {
            if (ray[i].collider.tag == "Wall")
            {
                return false;
            }

            if (ray[i].collider.tag == "Unit" && tile.moveTile)
            {
                return false;
            }
        }
        return true;
    }

    public virtual void Attack()
    {
        attackHitbox.position = currentSelectedMove.transform.position;

        for (int i = 0; i < attackHitbox.childCount; i++)
        {
            RaycastHit2D[] ray = Physics2D.RaycastAll(attackHitbox.GetChild(i).transform.position, Vector2.zero);

            for (int j = 0; j < ray.Length; j++)
            {
                if (ray[j].collider.tag == "Unit")
                {
                    ray[j].collider.GetComponent<Unit>().OnHit(attackDamage);
                }
            }
        }
    }

    public virtual void Move()
    {
        transform.position = GridManager.grid.RoundToGrid(currentSelectedMove.transform.position);
    }

    public virtual void AIMove()
    {
        currentSelectedMove = tiles[Random.Range(0, tiles.Length - 1)];
        TurnManager.tm.LockMove();
    }



}
