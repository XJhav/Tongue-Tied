using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SunkboxColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains("Lava"))
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color(0.85f, 0.56f, 0.5f);
        }
    }
}
