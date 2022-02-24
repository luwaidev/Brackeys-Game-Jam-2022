using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTileController : MonoBehaviour
{

    private SpriteRenderer sr;
    public Unit parent;
    public bool moveTile;


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
    public void Selected()
    {
        Debug.Log("Selected");
        parent.currentSelectedMove = this;
    }

}
