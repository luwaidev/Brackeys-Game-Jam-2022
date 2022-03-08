using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int index;
    public GameObject[] images;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            images[index].SetActive(false);
            index++;

            if (index >= images.Length)
            {
                GameManager.instance.Load("Level 1");

            }   else {
                images[index].SetActive(true);
            }

        }

        if (Input.GetKeyDown(KeyCode.Return)){
            GameManager.instance.LoadNext();
        }

    }
}
