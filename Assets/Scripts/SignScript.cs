using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignScript : MonoBehaviour
{
    [SerializeField] string dialogueText;
    GameObject dialogueBox;
    GameObject textObject;
    GameObject player;

    private void Start()
    {
        textObject = GameObject.FindGameObjectWithTag("Dialogue Text");
        dialogueBox = GameObject.FindGameObjectWithTag("Dialogue Box");
        player = GameObject.FindGameObjectWithTag("Player");
        textObject.GetComponent<Text>().text = dialogueText;
    }
    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= 1f)
        {
            dialogueBox.SetActive(true);
        }
        else
        {
            dialogueBox.SetActive(false);
        }
    }

}
