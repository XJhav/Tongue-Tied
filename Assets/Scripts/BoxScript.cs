using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA;

public class BoxScript : MonoBehaviour
{
    BoxCollider2D coll;
    FrogMovement pScript;
    BoxScript script;
    [SerializeField] GameObject splash;
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject sunkBox;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask blocks;
    [SerializeField] LayerMask boxGround;
    [SerializeField] LayerMask ice;
    public bool instantSink;
    public bool isFrozen;
    Vector3 pastPos;
    public bool moving = false;
    Vector3 currentPos;
    Vector3 finalPos;
    GameObject[] objectsToMove;
    bool beginMoving = false;
    SpriteRenderer rend;


    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rend = sprite.GetComponent<SpriteRenderer>();
        script = GetComponent<BoxScript>();
        pScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FrogMovement>();
        objectsToMove = objectsToMove = new GameObject[] { gameObject };
        pastPos = transform.position;
        currentPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeDepth();        
        BoxGroundCheck();
        CheckPoint();
        Move();

    } 
    void BoxGroundCheck()
    {
        bool onGround = Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.8f), 0f, Vector2.up, 0f, ground);
        if (!onGround && ((pScript.moving == false && pScript.tonguing == false) || instantSink == true))
        {
            Sink();
        }
    }
    void Sink()
    {
        pScript.objToPull = null;
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
        Instantiate(sunkBox, transform.position, Quaternion.identity);
        GameObject splashObject = Instantiate(splash, transform.position + Vector3.down * 0.3f, Quaternion.identity);
        if (SceneManager.GetActiveScene().name.Contains("Lava"))
        {
            splashObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.4f);
        }

        sprite.transform.localPosition = sprite.transform.localPosition = new Vector3(0f, -0.3f, 0f);
        GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("Splash");
        Destroy(gameObject);
    }
    public void IceCheck()
    {
        if (Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, ice) && !(Physics2D.BoxCast(transform.position - pScript.moveDir, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, blocks) && Physics2D.BoxCast(transform.position - pScript.moveDir, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, ice) && Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, ground)))
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
            currentPos = transform.position;
            finalPos = transform.position + pScript.moveDir;
            beginMoving = true;
        }
    }
    void ChangeDepth()
    {
        if (Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, boxGround))
        {
            sprite.transform.localPosition = new Vector3(0f, -0.15f, 0f);
        }
        else
        {
            sprite.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }
    void CheckPoint()
    {
        if (beginMoving)
        {
            int loopNum = 1;
            while (moving == false)
            {
                RaycastHit2D hit = Physics2D.BoxCast(transform.position + (pScript.moveDir * loopNum), new Vector2(0.8f, 0.8f), 0f, pScript.moveDir, 0f, blocks);

                if (hit.collider == null)
                {
                    beginMoving = false;
                    pScript.boxMoving = true;
                    moving = true;
                }
                else if (hit.collider.gameObject.tag == "Pushable")
                {
                    GameObject[] newArray = new GameObject[objectsToMove.Length + 1];
                    for (int i = 0; i < objectsToMove.Length; i++)
                    {
                        newArray[i] = objectsToMove[i];
                    }
                    newArray[newArray.Length - 1] = hit.collider.gameObject;
                    objectsToMove = newArray;
                    loopNum++;
                }
                else if (hit.collider.gameObject.tag == "Unpushable")
                {
                    beginMoving = false;
                    objectsToMove = objectsToMove = new GameObject[] { gameObject };
                    break;
                }

            }

        }
    }
    void Move()
    {
        if (moving == true && pScript.moving == false)
        {
            pScript.delaySinceMove = 0f;
            if (Vector3.Distance(transform.position, finalPos) >= 0.05f)
            {
                foreach (GameObject obj in objectsToMove)
                    if (obj != null)
                    {
                        obj.transform.position += pScript.moveDir * Time.deltaTime * 6f;
                    }
            }
            else
            {
                foreach (GameObject obj in objectsToMove)
                {
                    if (obj != null)
                    {
                        obj.transform.position = new Vector3(Mathf.Round(obj.transform.position.x), Mathf.Round(obj.transform.position.y), 0f);
                        if (obj.tag == "Pushable")
                        {
                            obj.GetComponent<BoxScript>().IceCheck();
                        }  
                    }
 
                }
                objectsToMove = objectsToMove = new GameObject[] { gameObject };
                pScript.boxMoving = false;
                moving = false;

            }
        }
    }

}
