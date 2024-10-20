using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour
{
    public string[] domains = new string[] {
       "https://www.coolmathgames.com",
       "www.coolmathgames.com",
       "https://edit.coolmathgames.com",
       "edit.coolmathgames.com",
       "www.stage.coolmathgames.com",
       "stage-edit.coolmathgames.com",
       "https://dev.coolmathgames.com",
       "https://www.stage.coolmathgames.com",
       "https://stage-edit.coolmathgames.com",
       "https://dev.coolmathgames.com",
       "https://dev-edit.coolmathgames.com"
   };
    [DllImport("__Internal")]
    private static extern void RedirectTo();

    [DllImport("__Internal")]
    private static extern void StartGameEvent();

    [DllImport("__Internal")]
    private static extern void StartLevelEvent(int level);

    [DllImport("__Internal")]
    private static extern void ReplayEvent(int level);

    private void CheckDomains()
    {
        if (!IsValidHost(domains))
        {
            RedirectTo();
        }
    }
    private bool IsValidHost(string[] hosts)
    {
        foreach (string host in hosts)
        {
            if (Application.absoluteURL.IndexOf(host) == 0)
            {
                return true;
            }
        }
        return false;
    }
    public void Redirect()
    {
      #if UNITY_WEBGL
      RedirectTo();
      #endif
    }

    public void StartGame()
    {
        #if UNITY_WEBGL
        StartGameEvent();
        #endif
    }


    public void StartLevel(int level)
    {
        #if UNITY_WEBGL
        StartLevelEvent(level);
        Redirect();
        #endif
    }

    public void Replay(int level)
    {
        #if UNITY_WEBGL
        ReplayEvent(level);
        #endif
    }

    private static bool hasStartedGame = false;


    public Animator transition;
    public bool dontLoadNext = false;
    public int nextSceneIndex;
    public float transitionTime = 1f;
    private const string CompletedLevelsKey = "CompletedLevels";
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);
        }

        if (!hasStartedGame)
        {
            StartGame();
            hasStartedGame = true;
        }
    }
    public void LoadNextLevel()
    {
        if (dontLoadNext)
        {
            StartCoroutine(LoadLevel(nextSceneIndex));
        }
        else
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }

    }
    public void LoadScene(int buildIndex)
    {
        StartCoroutine(LoadLevel(buildIndex));
    }

    IEnumerator LoadLevel(int buildIndex)
    {
        GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("Whoosh");
        transition.SetTrigger("Start Transition");
        yield return new WaitForSeconds(transitionTime);

        if (buildIndex != SceneManager.GetActiveScene().buildIndex)
        {
            StartLevel(buildIndex);
        }

        SceneManager.LoadScene(buildIndex);
    }
    public void Restart()
    {
        Replay(SceneManager.GetActiveScene().buildIndex);
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }
    public void MarkLevelCompleted(int levelIndex)
    {
        string completedLevelsString = PlayerPrefs.GetString(CompletedLevelsKey, "");
        string[] completedLevels = completedLevelsString.Split(',');

        if (Array.IndexOf(completedLevels, levelIndex.ToString()) == -1)
        {
            completedLevelsString += (completedLevelsString == "" ? "" : ",") + levelIndex;
            PlayerPrefs.SetString(CompletedLevelsKey, completedLevelsString);
        }
    }

    public bool IsLevelCompleted(int levelIndex)
    {
        string completedLevelsString = PlayerPrefs.GetString(CompletedLevelsKey, "");
        string[] completedLevels = completedLevelsString.Split(',');

        return Array.IndexOf(completedLevels, levelIndex.ToString()) != -1;
    }
    public int GetCompletedLevelsCount()
    {
        string completedLevelsString = PlayerPrefs.GetString(CompletedLevelsKey, "");

        if (string.IsNullOrEmpty(completedLevelsString))
        {
            return 0;
        }

        string[] completedLevels = completedLevelsString.Split(',');
        return completedLevels.Length;
    }
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }
}
