using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadUnit : Unit
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    // Called by attacks
    public override void OnHit(int damage)
    {
        Debug.Log("Hit");
        health -= damage;

        if (health <= 0)
        {
            anim.SetTrigger("Dead");
            Destroy(gameObject, 1);
            if (player == 0) GameManager.instance.Load(SceneManager.GetActiveScene().name);
            else GameManager.instance.LoadNext();
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

            if (units[i].player == 0)
            {
                closestPlayerUnit = (Vector2.Distance(transform.position, units[i].transform.position) <
                                     Vector2.Distance(transform.position, closestPlayerUnit)) ?
                                     (Vector2)units[i].transform.position : closestPlayerUnit;
            }
        }

        // Loop through own positions and run to furthest area
        MoveTileController furthest = tiles[0];
        for (int i = 0; i < tiles.Length; i++)
        {
            if (CheckTileValid(tiles[i]))
            {
                furthest = (Vector2.Distance(transform.position, tiles[i].transform.position) <
                            Vector2.Distance(transform.position, furthest.transform.position)) ?
                            tiles[i] : furthest;
            }
        }

        currentSelectedMove = furthest;

        TurnManager.tm.LockMove();
    }
}
