using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TurnManager : MonoBehaviour
{
    public bool devBuild;
    public static TurnManager tm;
    public Unit[] units;


    // is an index of a unit in the units list
    public int currentSelectedUnit;

    // 0 for player 1 for enemy
    public int currentPlayer;
    public bool moving;
    public float animationTime;

    [Header("UI")]
    public Image portrait;
    public Image bottomLeftUI;

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

    // Called to start game
    public void StartGame()
    {
        // Set new unit to selected
        units[currentSelectedUnit].SetActive();

        UpdateUIElements();
    }

    // Mostly checking for new unit to select and set it active
    public void OnNextTurn()
    {
        // Deselect old unit
        units[currentSelectedUnit].SetAsleep();

        // Check that all players are still alive
        bool hasPlayer0 = false;
        bool hasPlayer1 = true;
        for (int i = 0; i < units.Length; i++)
        {
            // Check that at least one instance of player and enemies head unit is alive
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
        if (!devBuild && !hasPlayer0 || !hasPlayer1)
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

        // Update UI Elements
        UpdateUIElements();
    }


    // Called by lock move button
    public void LockMove()
    {

        Debug.Log("Locked Move");
        StartCoroutine(LockCoroutine());
    }

    public IEnumerator LockCoroutine()
    {
        units[currentSelectedUnit].LockMove();
        yield return new WaitForSeconds(animationTime);
        OnNextTurn();
    }

    // Called by switch mode button
    public void SwitchMode()
    {
        moving = !moving;

        units[currentSelectedUnit].SetMode(moving);
    }

    public void UpdateUIElements()
    {
        portrait.sprite = units[currentSelectedUnit].portrait;
        bottomLeftUI.sprite = units[currentSelectedUnit].bottomLeftUI;
    }

    private void EndGame()
    {

        Debug.Log("Ended Game");
    }
}
