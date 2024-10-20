using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FrogMovement : MonoBehaviour
{
    public bool canInput = true;
    bool levelComplete = false;
    bool moveAnim = false;
    UIManager uiScript;
    UndoManager undoScript;
    BoxCollider2D coll;
    LineRenderer lrend;
    [SerializeField] GameObject sprite;
    public GameObject objToPull = null;
    Animator anim;
    SpriteRenderer srend;
    public GameObject[] objectsToMove;
    [SerializeField] GameObject splash;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask boxGround;
    [SerializeField] LayerMask blocks;
    [SerializeField] LayerMask ice;
    [SerializeField] LayerMask magma;
    [SerializeField] GameObject levelCompleteText;
    float moveSpeed = 6f;
    public bool moving = false;
    public bool tonguing = false;
    public bool boxMoving = false;
    bool beginMoving = false;
    public bool extending = false;
    public bool tongueStuck = false;
    public string faceDir;
    float lineOffset = 0f;
    Vector3 currentPos;
    Vector3 finalPos;
    public Vector3 tonguePos;
    public Vector3 moveDir;
    [HideInInspector]
    public bool unsunk = true;
    bool canMove = true;
    public float delaySinceMove = 0f;
    float minimumDelay = 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        anim = sprite.GetComponent<Animator>();
        srend = sprite.GetComponent<SpriteRenderer>();
        lrend = GetComponent<LineRenderer>();
        objectsToMove = objectsToMove = new GameObject[] { gameObject };
        uiScript = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        undoScript = GameObject.FindGameObjectWithTag("UndoManager").GetComponent<UndoManager>();
        tonguePos = transform.position;
        levelCompleteText = GameObject.FindGameObjectWithTag("Level Complete Stuff");
        levelCompleteText.SetActive(false);

        currentPos = transform.position;
        finalPos = transform.position + Vector3.right;
        moveDir = Vector3.right;
        faceDir = "Right";
        undoScript.SaveState();
    }

    // Update is called once per frame
    void Update()
    {
        delaySinceMove += Time.deltaTime;

        if (moving == true || boxMoving == true)
        {
            canMove = false;
        }
        if (canInput && !boxMoving && !levelComplete)
        {
            Inputs();
        }

        CheckPoint();
        Move();
        //IceCheck();
        SinkCheck();
        Tongue();
        TongueRend();
        Pull();
        Animate();
        if (!levelComplete)
        {
            InputPause();
        }
        ChangeDepth();
        if (moving == false || boxMoving == false)
        {
            canMove = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Star")
        {
            Destroy(collision.gameObject);
            levelComplete = true;
            StartCoroutine(EndSequence());
            GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("Star");
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().MarkLevelCompleted(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Move()
    {
        if (moving == true)
        {
            delaySinceMove = 0f;
            moveAnim = true;
            if (Vector3.Distance(transform.position, finalPos) >= 0.05f)
            {
                foreach (GameObject obj in objectsToMove)
                    if (obj != null)
                    {
                     obj.transform.position += moveDir * Time.deltaTime * 6f;
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

                moving = false;
                IceCheck();
                objectsToMove = objectsToMove = new GameObject[] { gameObject };
            }
        }
    }
    void Inputs()
    {
        if ((Input.GetKey(KeyCode.D) == true || Input.GetKey(KeyCode.RightArrow) == true) && canMove == true && tonguing == false && delaySinceMove >= minimumDelay)
        {
            if (GroundCheck(Vector3.right))
            {
                undoScript.SaveState();
                GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("PlayerMove");
                beginMoving = true;
                currentPos = transform.position;
                finalPos = transform.position + Vector3.right;
                moveDir = Vector3.right;
                faceDir = "Right";
                delaySinceMove = 0f;
            }

            
        }
        else if ((Input.GetKey(KeyCode.A) == true || Input.GetKey(KeyCode.LeftArrow) == true) && moving == false && tonguing == false && delaySinceMove >= minimumDelay)
        {
            if (GroundCheck(Vector3.left))
            {
                undoScript.SaveState();
                GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("PlayerMove");
                beginMoving = true;
                currentPos = transform.position;
                finalPos = transform.position + Vector3.left;
                moveDir = Vector3.left;
                faceDir = "Left";
                delaySinceMove = 0f;
            }

        }
        else if ((Input.GetKey(KeyCode.W) == true || Input.GetKey(KeyCode.UpArrow) == true) && moving == false && tonguing == false && delaySinceMove >= minimumDelay)
        {
            if (GroundCheck(Vector3.up))
            {
                undoScript.SaveState();
                GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("PlayerMove");
                beginMoving = true;
                currentPos = transform.position;
                finalPos = transform.position + Vector3.up;
                moveDir = Vector3.up;
                faceDir = "Up";
                delaySinceMove = 0f;
            }

        }
        else if ((Input.GetKey(KeyCode.S) == true || Input.GetKey(KeyCode.DownArrow) == true) && moving == false && tonguing == false && delaySinceMove >= minimumDelay)
        {
            if (GroundCheck(Vector3.down))
            {
                undoScript.SaveState();
                GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("PlayerMove");
                beginMoving = true;
                currentPos = transform.position;
                finalPos = transform.position + Vector3.down;
                moveDir = Vector3.down;
                faceDir = "Down";
                delaySinceMove = 0f;
            }

        }
        else if (Input.GetKeyDown(KeyCode.Space) == true && moving == false && tonguing == false && delaySinceMove >= 0.05f)
        {
            undoScript.SaveState();
            GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("Tongue");
            tonguing = true;
            extending = true;
        }

    }
    void Animate()
    {
        if (moveAnim)
        {
            anim.SetBool("Jumping", true);
        }
        else
        {
            anim.SetBool("Jumping", false);
        }

        if (faceDir == "Right" || faceDir == "Left")
        {
            anim.SetInteger("FacingUp", 0);
            if (faceDir == "Left")
            {
                srend.flipX = true;
            }
            else
            {
                srend.flipX = false;
            }
        }
        else if (faceDir == "Up")
        {
            anim.SetInteger("FacingUp", 1);
        }
        else if (faceDir == "Down")
        {
            anim.SetInteger("FacingUp", -1);
        }
    }
    void CheckPoint()
    {
        if (beginMoving)
        {
            int loopNum = 1;
            while (moving == false)
            {
                RaycastHit2D hit = Physics2D.BoxCast(transform.position + (moveDir * loopNum), new Vector2(0.8f, 0.8f), 0f, moveDir, 0f, blocks);

                if (hit.collider == null)
                {
                    beginMoving = false;
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
                    moveAnim = false;
                    break;
                }
                
            }
            
        }
    }
    void Tongue()
    {
        if (tonguing)
        {
            if (extending)
            {
                tonguePos += moveDir * Time.deltaTime * moveSpeed * 2f;
            }
            else if (extending == false && tongueStuck == false)
            {
                tonguePos -= moveDir * Time.deltaTime * moveSpeed * 2f;
            }
            else if (tongueStuck && Vector3.Distance(transform.position, tonguePos) >= 1)
            {
                transform.position += moveDir * Time.deltaTime * moveSpeed * 2f;
            }
            else if (tongueStuck)
            {
                tongueStuck = false;
                transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
            }
            if (Vector3.Distance(transform.position, tonguePos) >= 4f)
            {
                extending = false;
            }
            if (Vector3.Distance(transform.position, tonguePos) <= 0.2f && extending == false)
            {
                tonguePos = transform.position;
                tonguing = false;
            }
        }
        else
        {
            tonguePos = transform.position;
        }
    }
    void TongueRend()
    {
        if (faceDir == "Left" || faceDir == "Right")
        {
            lrend.startWidth = 0.0625f;
            lrend.endWidth = 0.0625f;
            lrend.sortingOrder = 5;
        }
        else if (faceDir == "Up")
        {
            lrend.startWidth = 0.125f;
            lrend.endWidth = 0.125f;
            lrend.sortingOrder = 3;
        }
        else if (faceDir == "Down")
        {
            lrend.startWidth = 0.125f;
            lrend.endWidth = 0.125f;
            lrend.sortingOrder = 5;
        }

        lrend.SetPosition(0, transform.position + Vector3.up * lineOffset);
        lrend.SetPosition(1, tonguePos + Vector3.up * lineOffset);
    }
    void Pull()
    {
        if (objToPull != null)
        {
            if (Vector3.Distance(tonguePos, transform.position) >= 1 && objToPull.GetComponent<BoxScript>().isFrozen == false)
            {
                objToPull.transform.position = tonguePos;
            }
            else
            {
                objToPull.transform.position = new Vector3(Mathf.Round(objToPull.transform.position.x), Mathf.Round(objToPull.transform.position.y), 0f);
                objToPull = null;
            }
        }
    }
    bool GroundCheck(Vector3 direction)
    {
        return (Physics2D.BoxCast(transform.position + direction, new Vector2(0.8f, 0.8f), 0f, direction, 0f, ground));
    }
    void InputPause()
    {
        if (Input.GetKeyDown(KeyCode.R) == true)
        {
            GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().Restart();
        }
        else if (Input.GetKeyDown(KeyCode.Z) == true && moving == false && tonguing == false && uiScript.paused == false)
        {
            undoScript.Undo();
            unsunk = true;
            canInput = true;
            sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Q) == true)
        {
            uiScript.MainMenuFunction();
        }
    }
    void ChangeDepth()
    {
        if (Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, boxGround))
        {
            sprite.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            lineOffset = -0.15f;
        }
        else
        {
            sprite.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            lineOffset = 0f;
        }
    }
    void IceCheck()
    {
        if (Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, ice) && moving == false)
        {
            canMove = false;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
            currentPos = transform.position;
            finalPos = transform.position + moveDir;
            beginMoving = true;
        }
        else
        {
            moveAnim = false;
        }
    }
    void SinkCheck()
    {
        if (!Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.8f), 0f, Vector2.up, 0f, ground) && unsunk == true && tonguing == false && !Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.8f), 0f, Vector2.up, 0f, magma))
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
            sprite.GetComponent<SpriteRenderer>().color = new Color (1f, 1f, 1f, 0f);
            GameObject splashObject = Instantiate(splash, transform.position, Quaternion.identity);
            if (SceneManager.GetActiveScene().name.Contains("Lava"))
            {
                splashObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.4f);
            }
            GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("Splash");
            unsunk = false;           
            canInput = false;
        }
        if (!Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.8f), 0f, Vector2.up, 0f, ground) && unsunk == true && tonguing == false && Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.8f), 0f, Vector2.up, 0f, magma))
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
            sprite.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f);
            GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>().Play("Burn");
            unsunk = false;
            canInput = false;
        }
    }
    IEnumerator EndSequence()
    {
        levelCompleteText.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelScript>().LoadNextLevel();
    }


}
