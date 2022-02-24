using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurnManager : MonoBehaviour
{
    public static TurnManager tm;
    public Unit[] units;


    public int currentSelectedUnit;
    public int currentPlayer;

    public bool moving;
    // Start is called before the first frame update
    void Start()
    {
        tm = this;

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void StartGame()
    {
        // Set new unit to selected
        units[currentSelectedUnit].SetActive();
    }
    public void OnNextTurn()
    {
        // Deselect old unit
        units[currentSelectedUnit].SetAsleep();

        // Check that all players are still alive
        bool hasPlayer0 = false;
        bool hasPlayer1 = true;
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].player == 0 && units[i].GetComponent<HeadUnit>() != null)
            {
                hasPlayer0 = true;
            }
            else if (units[i].player == 1 && units[i].GetComponent<HeadUnit>() != null)
            {
                hasPlayer1 = true;
            }
        }

        // End game if player is dead
        if (!hasPlayer0 || !hasPlayer1)
        {
            EndGame();
            return;
        }

        // Find next unit 
        for (int i = currentSelectedUnit; i < units.Length; i++)
        {

            // Check not current player
            if (units[i].player == currentPlayer && i != currentSelectedUnit)
            {
                currentSelectedUnit = i;
                break;
            }
            // Check if at end of players units
            else if (i == units.Length - 1)
            {
                Debug.Log("Switching Players");
                currentPlayer = currentPlayer == 0 ? 1 : 0;

                // Find new unit from other player
                for (int j = 0; j < units.Length; j++)
                {
                    if (units[j].player == currentPlayer)
                    {
                        currentSelectedUnit = j;
                        break;
                    }
                }
                break;
            }
        }
        moving = true;

        // Set new unit to selected
        units[currentSelectedUnit].SetActive();
    }


    public void LockMove()
    {

        Debug.Log("Locked Move");
        units[currentSelectedUnit].LockMove();
        OnNextTurn();
    }

    public void SwitchMode()
    {
        moving = !moving;

        units[currentSelectedUnit].SetMode(moving);
    }

    private void EndGame()
    {

        Debug.Log("Ended Game");
    }
}
