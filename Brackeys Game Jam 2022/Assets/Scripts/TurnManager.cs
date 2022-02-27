using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TurnManager : MonoBehaviour
{
    public bool devBuild;
    public GameObject enemyTurn;
    public static TurnManager tm;
    public Unit[] units;
    public int currentTurn;
    public bool locking;


    // is an index of a unit in the units list
    public int currentSelectedUnit;

    // 0 for player 1 for enemy
    public int currentPlayer;
    public bool moving;
    public float animationTime;
    public float aiWaitTime;

    [Header("UI")]
    public Image portrait;
    public Image bottomLeftUI;

    [Header("Hacking Settings")]
    public int maxHackingTime;
    public int hackingTimer;
    public Animator hackIndicator;
    public TMP_Text turnIndicator;
    public int hackAmount;
    public bool playHackAnimation;
    public bool playHackWarning;


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
            if (units[i] != null && units[i].player == 0 && units[i].GetComponent<HeadUnit>() != null)
            {
                hasPlayer0 = true;
            }
            else if (units[i] != null && units[i].player == 1 && units[i].GetComponent<HeadUnit>() != null)
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
                // Debug.Log("Switching Players");
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


                if (units[i].player == 1) currentTurn++;
                break;
            }
        }
        moving = true;

        // Set new unit to selected
        units[currentSelectedUnit].SetActive();


        // Update UI Elements
        UpdateUIElements();

    }

    public void Hack()
    {
        // Hacking
        if (hackingTimer == 0)
        {
            Debug.Log("Hacking");
            // Do hack
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].hacking)
                {
                    units[i].player = units[i].player == 0 ? 1 : 0;
                    units[i].anim.SetBool("IsPlayer", units[i].player == 0);
                    units[i].hackDisplay.SetTrigger("Hacking");
                    units[i].hacking = false;
                    playHackAnimation = true;
                }

            }
            hackIndicator.SetTrigger("Hacking");

            hackingTimer = maxHackingTime * units.Length - (Random.Range(0, 2));
        }
        else if (hackingTimer == 1)
        {
            // Choose hackmans

            for (int i = 0; i < hackAmount; i++)
            {
                // Find player units
                for (int j = 0; j < units.Length; j++)
                {
                    if (units[j] == null || units[j].GetComponent<HeadUnit>() != null || units[j].hacking)
                    {
                        continue;
                    }

                    if (units[j].player == 0)
                    {
                        units[j].hacking = true;
                        units[j].hackDisplay.SetTrigger("Hacking");
                        playHackWarning = true;
                        break;
                    }
                }

                //Find enemy units
                for (int j = 0; j < units.Length; j++)
                {
                    if (units[i] == null || units[j].GetComponent<HeadUnit>() != null || units[j].hacking)
                    {
                        continue;
                    }

                    if (units[j].player == 1)
                    {
                        units[j].hacking = true;
                        units[j].hackDisplay.SetTrigger("Hacking");
                        playHackWarning = true;
                        break;
                    }
                }
            }
            // Show hacking
            hackIndicator.SetTrigger("Warning");
            hackingTimer--;
        }
        else
        {
            hackingTimer--;
        }
    }

    // Called by lock move button
    public void LockMove()
    {

        // Debug.Log("Locked Move");
        if (!locking) StartCoroutine(LockCoroutine());
    }

    public IEnumerator LockCoroutine()
    {
        locking = true;
        units[currentSelectedUnit].LockMove();
        yield return new WaitForSeconds(animationTime);
        if (units[currentSelectedUnit].player == 1)
        {
            print("bruh");
            yield return new WaitForSeconds(aiWaitTime);
        }

        Hack();

        if (playHackWarning)
        {
            yield return new WaitForSeconds(2);
            playHackWarning = false;
        }
        else if (playHackAnimation)
        {

            yield return new WaitForSeconds(6);
            playHackAnimation = false;
        }


        locking = false;
        OnNextTurn();

    }

    // Called by switch mode button
    public void SwitchMode()
    {
        if (units[currentSelectedUnit].player == 0)
        {
            moving = !moving;

            units[currentSelectedUnit].SetMode(moving);

            UpdateUIElements();
        }
    }

    public void UpdateUIElements()
    {
        // Update portraits
        Unit unit = units[currentSelectedUnit];
        portrait.sprite = unit.portrait;
        bottomLeftUI.sprite = moving ? unit.bottomLeftUI : unit.bottomLeftUISelected;

        // Update turn
        turnIndicator.text = currentTurn.ToString();

        // Update Enemy turn
        enemyTurn.SetActive(units[currentSelectedUnit].player == 1);

    }

    private void EndGame()
    {

        Debug.Log("Ended Game");
    }
}
