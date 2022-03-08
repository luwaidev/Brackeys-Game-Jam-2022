using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public int currentLevel;
    public Sprite[] icons;
    public Image icon;
    public GameObject levelMenu;
    public GameObject openScreen;
    public void PlayButton()
    {
        levelMenu.SetActive(true);
        openScreen.SetActive(false);
    }


    public void StartGameButton()
    {
        GameManager.instance.Load("Tutorial");
    }

    public void ChangeLevel(int level)
    {
        currentLevel = level;
        if (icons[currentLevel] != null) icon.sprite = icons[currentLevel];
    }
}
