using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
Every possible attack and move has a move tile script

*/
public class MoveTileController : MonoBehaviour
{

    private SpriteRenderer sr;

    // Parent unit
    public Unit parent;

    // Used to distinguish if this tile is for attacking or moving
    public bool moveTile;

    // Don't need to use these for final version, can have fancy animations if you want
    // But for now it looks pretty decent
    [Header("Temp Colors")]
    public Color idleColor;
    public Color selectedColor;
    public SpriteRenderer[] subTiles;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sr.enabled = parent.player == 0;
        // When mouse is clicked check if mouse is over this object
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hit;
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.RaycastAll(ray, Vector2.zero);
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i] && hit[i].transform.name == gameObject.name)
                {
                    Selected();
                    break;
                }
            }
        }

        if (parent.currentSelectedMove == this)
        {
            sr.color = selectedColor;
            if (subTiles.Length != 0)
            {
                for (int i = 0; i < subTiles.Length; i++)
                {
                    subTiles[i].color = selectedColor;
                }
            }

        }
        else
        {
            sr.color = idleColor;

            if (subTiles.Length != 0)
            {
                for (int i = 0; i < subTiles.Length; i++)
                {
                    subTiles[i].color = idleColor;
                }
            }
        }
    }

    // Set current selected move in the parent Unit script to this
    public void Selected()
    {
        // Debug.Log("Selected");

        if (!parent.moved && moveTile && parent.currentSelectedMove == this)
        {
            // Debug.Log("Move Unit");
            parent.MoveUnit();
        }
        if (!moveTile && parent.currentSelectedMove == this)
        {
            // Debug.Log("Move Unit");
            TurnManager.tm.LockMove();
        }
        else if (parent.moved)
        {
            // Debug.Log("Setting Unit Moved");
            parent.currentSelectedMove = moveTile ? null : this;
        }
        else if (!parent.moved)
        {
            // Debug.Log("Setting Unit Not Moved");
            parent.currentSelectedMove = this;
        }
    }

}
