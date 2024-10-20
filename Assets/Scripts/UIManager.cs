using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject lostText;
    [SerializeField] Image volumeBar;
    [SerializeField] AudioMixer mainMixer;
    public bool paused = false;
    float sfxVolume = 1f;
    float musicVolume = 1f;
    FrogMovement pScript;

    private void Start()
    {
        pScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FrogMovement>();
        lostText = GameObject.FindGameObjectWithTag("LostText");
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
        pScript.canInput = false;
        paused = true;
        GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().PauseMusic();
    }

    public void UnPause()
    {
        pausePanel.SetActive(false);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        pScript.canInput = true;
        paused = false;
        GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().UnpauseMusic();
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        pScript.canInput = false;
    }

    public void ChangeSFXVolume(float amount)
    {
        sfxVolume += amount;
        if (sfxVolume < 0)
        {
            sfxVolume = 0;
        }
        if (sfxVolume > 1)
        {
            sfxVolume = 1;
        }
        volumeBar.fillAmount = sfxVolume;
        mainMixer.SetFloat("SFXVolume", (sfxVolume - 1) * 80);
    }
    public void ChangeMusicVolume(float amount)
    {
        musicVolume += amount;
        if (musicVolume < 0)
        {
            musicVolume = 0;
        }
        if (musicVolume > 1)
        {
            musicVolume = 1;
        }
        volumeBar.fillAmount = musicVolume;
        mainMixer.SetFloat("MusicVolume", (musicVolume - 1) * 80);
    }
    public void Restart()
    {
        GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().UnpauseMusic();
        GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().Restart();
    }
    public void SceneLoad(int buildIndex)
    {
        GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().UnpauseMusic();
        GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(buildIndex);
    }
    private void Update()
    {
        if (pScript.unsunk == false)
        {
            pausePanel.SetActive(true);
            lostText.SetActive(true);
        }
        else
        {
            pausePanel.SetActive(false);
            lostText.SetActive(false);
        }
    }
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }
    public void MainMenuFunction()
    {
        GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().UnpauseMusic();

        if (SceneManager.GetActiveScene().name.Contains("Grass"))
        {
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(2);
        }
        else if (SceneManager.GetActiveScene().name.Contains("Swamp"))
        {
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(3);
        }
        else if (SceneManager.GetActiveScene().name.Contains("Snow"))
        {
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(4);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(1);
        }

    }
    public void ReqSceneLoad(int buildIndex)
    {
        if (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().GetCompletedLevelsCount() >= 8 && buildIndex == 3)
        {
            GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().UnpauseMusic();
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(buildIndex);
        }
        else if (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().GetCompletedLevelsCount() >= 16 && buildIndex == 4)
        {
            GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().UnpauseMusic();
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(buildIndex);
        }
        else if (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().GetCompletedLevelsCount() >= 24 && buildIndex == 5)
        {
            GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().UnpauseMusic();
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(buildIndex);
        }
        else if (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().GetCompletedLevelsCount() >= 32 && buildIndex == 6)
        {
            GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().UnpauseMusic();
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadScene(buildIndex);
        }

    }
}
