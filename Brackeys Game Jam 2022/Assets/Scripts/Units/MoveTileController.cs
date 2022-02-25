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

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        // When mouse is clicked check if mouse is over this object
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit;
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(ray, Vector2.zero);
            if (hit && hit.transform.name == gameObject.name) Selected();
        }

        if (parent.currentSelectedMove == this)
        {
            sr.color = selectedColor;
        }
        else
        {
            sr.color = idleColor;
        }
    }

    // Set current selected move in the parent Unit script to this
    public void Selected()
    {
        Debug.Log("Selected");

        if (!parent.moved && moveTile && parent.currentSelectedMove == this)
        {
            parent.MoveUnit();
        }
        else if (parent.moved)
        {
            parent.currentSelectedMove = moveTile ? null : this;
        }
        else if (!parent.moved)
        {
            parent.currentSelectedMove = this;
        }
    }

}
