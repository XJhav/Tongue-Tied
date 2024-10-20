using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoaderScript : MonoBehaviour
{
    [SerializeField] int levelNum;
    [SerializeField] int[] previousLevels;
    LevelScript lScript;
    [SerializeField] GameObject textObject;
    [SerializeField] GameObject blueCircle;
    [SerializeField] GameObject goldCircle;
    [SerializeField] GameObject[] objectsToEnable;
    [SerializeField] GameObject[] objectsToDisable;
    [SerializeField] bool matchText = true;
    [SerializeField] string altText = "";
    [SerializeField] int sceneLevelOffset = 0;
    bool canLoad = false;
    bool unloaded = true;
    Text text;

    void Awake()
    {
        lScript = GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>();
        text = textObject.GetComponent<Text>();
    }
    private void Start()
    {
        foreach(GameObject obj in objectsToEnable)
        {
            obj.SetActive(false);
        }

        if (matchText)
        {
            text.text = levelNum.ToString();
        }
        else
        {
            text.text = altText;
        }

        foreach (int level in previousLevels)
        {
            if (level == 0 || lScript.IsLevelCompleted(level + sceneLevelOffset))
            {
                canLoad = true;
                foreach(GameObject obj in objectsToEnable)
                {
                    obj.SetActive(true);
                }
                foreach(GameObject obj in objectsToDisable)
                {
                    obj.SetActive(false);
                }
            }
        }

        if (lScript.IsLevelCompleted(levelNum + sceneLevelOffset))
        {
            blueCircle.SetActive(false);
            goldCircle.SetActive(true);
        }
    }
    public void Load()
    {
        blueCircle.GetComponent<Image>().color = new Color(0.5f, .5f, .5f);
        goldCircle.GetComponent<Image>().color = new Color(0.5f, .5f, .5f);
        textObject.GetComponent<Text>().color = new Color(0.5f, .5f, .5f);
        unloaded = false;
        if (canLoad)
        {
            lScript.LoadScene(levelNum + sceneLevelOffset);
        }

    }
    private void Update()
    {
        if (unloaded)
        {
            if ((Mathf.Abs((Camera.main.ScreenToWorldPoint(Input.mousePosition).x) - transform.position.x) <= 0.4f) && (Mathf.Abs((Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - transform.position.y) <= 0.4f))
            {
                blueCircle.GetComponent<Image>().color = new Color(0.75f, .75f, .75f);
                goldCircle.GetComponent<Image>().color = new Color(0.75f, .75f, .75f);
                textObject.GetComponent<Text>().color = new Color(0.75f, .75f, .75f);
            }
            else
            {
                blueCircle.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                goldCircle.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                textObject.GetComponent<Text>().color = new Color(1f, 1f, 1f);
            }
        }

    }
}
