using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedScript : MonoBehaviour
{
    FrogMovement pScript;

    void Start()
    {
        pScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FrogMovement>();
    }

    void Update()
    {
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (pScript.tonguing == false)
            {
                if (collision.tag == "Pushable" || collision.tag == "Player")
                {
                    GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("Rock Crack");
                    Destroy(gameObject);
                }
            }
            else if (pScript.tonguing == true)
            {
                if (collision.tag == "Pushable")
                {
                    if (collision.GetComponent<BoxScript>().instantSink == true)
                    {
                        GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("Rock Crack");
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
