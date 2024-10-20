using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockedWorld : MonoBehaviour
{
    [SerializeField] int requirement;
    [SerializeField] GameObject text;

    private void Start()
    {
        text.GetComponent<Text>().text = "x" + requirement.ToString();
        if (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().GetCompletedLevelsCount() >= requirement)
        {
            gameObject.SetActive(false);
        }
    }
}
