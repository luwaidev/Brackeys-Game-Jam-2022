using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public Animator transition;

    [Header("Scene Loading Settings")]
    [SerializeField] float sceneTransitionTime;
    public bool loaded;
    public bool loadingScene;
    public AudioSource sound;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("GameController") != gameObject) Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
    }
    public void Load(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    public void LoadNext()
    {
        string sceneName;
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            sceneName = "Level 3";
        }
        else if (SceneManager.GetActiveScene().name == "Level 3")
        {
            sceneName = "Level 4";
        }
        // else if (SceneManager.GetActiveScene().name == "Level 4")
        // {
        //     sceneName = "Level 5";
        // }
        else
        {
            sceneName = "Main Menu";
        }
        StartCoroutine(LoadScene(sceneName));
    }


    public void LoadWithDelay(string sceneName, float delayTime)
    {
        StartCoroutine(Delay(sceneName, delayTime));
    }

    IEnumerator Delay(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(LoadScene(sceneName));
    }
    public IEnumerator LoadScene(string sceneName)
    {
        if (loadingScene) yield break;
        loadingScene = true;

        // sound.Play();
        transition.SetTrigger("In"); // Start transitioning scene out
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition

        // Start loading scene
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;
        while (!load.isDone)
        {
            if (load.progress >= 0.9f)
            {
                load.allowSceneActivation = true;
            }

            yield return null;
        }
        load.allowSceneActivation = true;

        if (sceneName == "Menu")
        {

            Cursor.lockState = CursorLockMode.None;
        }
        else if (sceneName == "Game")
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        transition.SetTrigger("In"); // Start transitioning scene back

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        loadingScene = false;

        yield return new WaitForSeconds(1);
        instance = this;

        loaded = true;
    }

    private void Update()
    {

        // if (Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }


}
