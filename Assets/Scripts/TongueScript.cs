using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueScript : MonoBehaviour
{
    FrogMovement pScript;
    BoxCollider2D coll;
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        pScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FrogMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (collision.gameObject.GetComponent<BoxScript>().isFrozen)
            {
                pScript.tongueStuck = true;
            }
            pScript.extending = false;
            pScript.objToPull = collision.gameObject;

        }
        else if (collision.gameObject.layer == 6)
        {
            pScript.extending = false;
        }
    }
    void Update()
    {
        transform.position = pScript.tonguePos;
    }
}
