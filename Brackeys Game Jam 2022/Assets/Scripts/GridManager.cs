using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager grid;
    public Vector2 gridSize;
    public Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        grid = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Round position to isometric grid
    public Vector2 RoundToGrid(Vector2 position)
    {
        // Find Y value
        float y = position.y / gridSize.y;
        float x = position.x / gridSize.x;
        bool onEven = Mathf.Round(y) % 2 == 0;

        position.y = Mathf.Round(y) * gridSize.y;
        position.x = (onEven ? Mathf.Floor(x) : Mathf.Round(x)) * gridSize.x + (onEven ? gridSize.x / 2 : 0);
        return position + offset;
    }
}
