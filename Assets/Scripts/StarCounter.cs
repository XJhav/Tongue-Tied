using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StarCounter : MonoBehaviour
{

    
    void Update()
    {
        GetComponent<Text>().text = (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().GetCompletedLevelsCount().ToString());
    }
}
