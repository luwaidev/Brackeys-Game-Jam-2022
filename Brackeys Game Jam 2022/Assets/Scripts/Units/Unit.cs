using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

// This class can be inherited from by any Unit, and the code can be overriden for custom functionality
public abstract class Unit : MonoBehaviour
{
    bool selected;
    public MoveTileController[] tiles;
    public MoveTileController currentSelectedMove;
    public SpriteRenderer sprite;
    public Animator anim;
    public Animator hackDisplay;
    public bool hacking;
    public int player;
    public bool moved;
    public bool dead;

    [Header("Combat Settings")]
    public bool attack;

    public Transform attackHitbox;
    public int attackDamage;
    public int health;
    public int maxHealth;
    public float healthBarLerpSpeed = 0.125f;
    public RectTransform healthBar;

    [Header("Animation Timing")]
    public float moveTime;
    public float moveSpeed;

    [Header("UI Settings")]
    public Sprite bottomLeftUI;
    public Sprite portrait;
    public Sprite bottomLeftUISelected;
    public Sprite portraitSelected;

    private void Awake()
    {
        // sprite = GetComponent<SpriteRenderer>();
        // anim = GetComponent<Animator>();
        anim.SetBool("IsPlayer", player == 0);
        healthBar = GameObject.Find("Health Bar").GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (selected) healthBar.localScale = Vector2.Lerp(healthBar.localScale, new Vector2((float)health / (float)maxHealth, 1), healthBarLerpSpeed);
    }
    // Called by TurnManager
    public virtual void SetActive()
    {
        if (dead) return;
        currentSelectedMove = null;
        selected = true;
        moved = false;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null) tiles[i].gameObject.SetActive(tiles[i].moveTile && CheckTileValid(tiles[i]));
            // print(i + ", " + tiles[i].moveTile);
        }

        UpdateUnit();

        if (player == 1 && !attack)
        {
            AIMove();
        }

        if (attack)
        {
            Attack();
        }


    }

    public virtual void SetAsleep()
    {
        if (dead) return;
        currentSelectedMove = null;
        selected = false;
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].gameObject.SetActive(false);
        }
    }


    public virtual void LockMove()
    {
        if (dead) return;
        if (currentSelectedMove == null) return;

        Transform st = sprite.transform;
        st.localScale = new Vector2(Mathf.Abs(st.localScale.x) * Mathf.Sign(st.position.x - currentSelectedMove.transform.position.x), st.localScale.y);

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
        if (dead) return;
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
        if (dead) return;
        Debug.Log("Hit");
        health -= damage;

        if (health <= 0)
        {
            dead = true;
            anim.SetTrigger("Dead");
            Destroy(gameObject, 1);
        }
    }

    public virtual void MoveUnit()
    {
        if (dead) return;
        Move();
        moved = true;
        TurnManager.tm.moving = false;
    }
    // Private

    public virtual void UpdateUnit()
    {

    }
    // For now just checks if tile is not over another enemy or outside the map
    // Can be customized for every unit, maybe some can't fire over walls etc
    public virtual bool CheckTileValid(MoveTileController tile)
    {
        if (dead) return false;
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
        if (dead) return;
        // if (currentSelectedMove == null) return;
        // Placing hitbox
        attackHitbox.position = currentSelectedMove.transform.position;

        // Loop though and raycast hitbox area
        for (int i = 0; i < attackHitbox.childCount; i++)
        {
            RaycastHit2D[] ray = Physics2D.RaycastAll(attackHitbox.GetChild(i).transform.position, Vector2.zero);
            print(ray.Length);
            for (int j = 0; j < ray.Length; j++)
            {
                if (ray[j].collider.tag == "Unit")
                {
                    ray[j].collider.GetComponent<Unit>().OnHit(attackDamage);
                }
            }
        }

        // Do effect
        GameObject.FindGameObjectWithTag("Attack Feedback").GetComponent<MMFeedbacks>().PlayFeedbacks();
        currentSelectedMove = null;
        attack = false;
    }

    // Called when locking move
    public virtual void Move()
    {
        StartCoroutine(MoveRoutine());

    }

    IEnumerator MoveRoutine()
    {
        if (dead) yield break;
        // Set direction
        Transform st = sprite.transform;
        st.localScale = new Vector2(Mathf.Abs(st.localScale.x) * Mathf.Sign(st.position.x - currentSelectedMove.transform.position.x), st.localScale.y);

        Vector2 targetPosition = GridManager.grid.RoundToGrid(currentSelectedMove.transform.position);

        // Set tiles to false
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < moveTime; i++)
        {

            // Move to the tile selected's position
            transform.position = Vector2.Lerp(transform.position, targetPosition, moveSpeed);
            yield return new WaitForSeconds(0.01f);
        }

        // Move to the tile selected's position
        transform.position = targetPosition;


        currentSelectedMove = null;


        if (moved) SetMode(false);

        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tiles[i].moveTile)
            {
                yield break;
            }
        }
        if (player != 1) TurnManager.tm.LockMove();
    }


    // No ai rn it just chooses a random direction
    // But ai code would go here
    public virtual void AIMove()
    {
        if (dead) return;
        // Open all possible tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].gameObject.SetActive(CheckTileValid(tiles[i]));
        }

        currentSelectedMove = null;
        while (currentSelectedMove == null)
        {
            currentSelectedMove = tiles[Random.Range(0, tiles.Length - 1)];
            if (!currentSelectedMove.gameObject.activeInHierarchy)
            {
                currentSelectedMove = null;
            }
        }

        TurnManager.tm.LockMove();
    }



}
