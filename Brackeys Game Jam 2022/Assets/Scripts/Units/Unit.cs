using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class can be inherited from by any Unit, and the code can be overriden for custom functionality
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

    // Called by TurnManager
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

    // Switch which tiles are being shown
    public virtual void SetMode(bool moving)
    {
        // Loop through each tile, check if the tile is valid (see CheckTileValid for what that means)
        // Also check to switch to either all attacking or moving tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].gameObject.SetActive(tiles[i].moveTile == moving && CheckTileValid(tiles[i]));
        }
    }

    // Called by attacks
    public virtual void OnHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Private

    // For now just checks if tile is not over another enemy or outside the map
    // Can be customized for every unit, maybe some can't fire over walls etc
    public virtual bool CheckTileValid(MoveTileController tile)
    {
        RaycastHit2D[] ray = Physics2D.RaycastAll(tile.transform.position, Vector2.zero);

        for (int i = 0; i < ray.Length; i++)
        {
            if (ray[i].collider.tag == "Wall")
            {
                return false;
            }

            // Checking if on a unit is only valid if the tile isn't an attack tile
            if (ray[i].collider.tag == "Unit" && tile.moveTile)
            {
                return false;
            }
        }
        return true;
    }

    // Called when locking move
    // Place a preset hitbox at position, check enemies in range and do damage to them
    public virtual void Attack()
    {
        // Placing hitbox
        attackHitbox.position = currentSelectedMove.transform.position;

        // Loop though and raycast hitbox area
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

    // Called when locking move
    public virtual void Move()
    {
        // Move to the tile selected's position
        transform.position = GridManager.grid.RoundToGrid(currentSelectedMove.transform.position);
    }


    // No ai rn it just chooses a random direction
    // But ai code would go here
    public virtual void AIMove()
    {
        currentSelectedMove = tiles[Random.Range(0, tiles.Length - 1)];
        TurnManager.tm.LockMove();
    }



}
