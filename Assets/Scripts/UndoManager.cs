using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public List<GameState> states = new List<GameState> ();
    GameObject player;
    FrogMovement pScript;

    [SerializeField] GameObject box;
    [SerializeField] GameObject metalBox;
    [SerializeField] GameObject frozenBox;
    [SerializeField] GameObject noTongueBox;
    [SerializeField] GameObject sunkBox;
    [SerializeField] GameObject sunkMetalBox;
    [SerializeField] GameObject sunkFrozenBox;
    [SerializeField] GameObject sunkNoTongueBox;
    [SerializeField] GameObject cracked;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pScript = player.GetComponent<FrogMovement>();
    }
    public void SaveState()
    {
        if (player != null)
        {
            if (pScript.unsunk == true)
                    {
                        GameState state = new GameState();
                        state.playerPos = player.transform.position;
                        state.pMoveDir = pScript.moveDir;
                        state.pFaceDir = pScript.faceDir;
                        GameObject[] pushables = GameObject.FindGameObjectsWithTag("Pushable");

                        foreach (GameObject obj in pushables)
                        {
                            if (obj.GetComponent<BoxScript>().instantSink)
                            {
                                state.metalBoxes.Add(obj.transform.position);
                            }
                            else if (obj.GetComponent<BoxScript>().isFrozen)
                            {
                                state.frozenBoxes.Add(obj.transform.position);
                            }
                            else if (obj.layer == 6)
                            {
                                state.noTongueBoxes.Add(obj.transform.position);
                            }
                            else
                            {
                                state.boxes.Add(obj.transform.position);
                            }
                        }

                        GameObject[] sunks = GameObject.FindGameObjectsWithTag("New Ground");

                        foreach (GameObject obj in sunks)
                        {
                            if (obj.name == "MetalSunkBox Variant(Clone)")
                            {
                                state.sunkMetalBoxes.Add(obj.transform.position);
                            }
                            else if (obj.name == "Frozen SunkBox Variant(Clone)")
                            {
                                state.sunkFrozenBoxes.Add(obj.transform.position);
                            }
                            else if (obj.name == "NoTongueSunkBox(Clone)")
                            {
                                state.sunkNoTongueBoxes.Add(obj.transform.position);
                            }
                            else
                            {
                                state.sunkBoxes.Add(obj.transform.position);
                            }
                        }

                        GameObject[] crackeds = GameObject.FindGameObjectsWithTag("Cracked");

                        foreach (GameObject obj in crackeds)
                        {
                            state.crackedGround.Add(obj.transform.position);
                        }

                        states.Add(state);
            }
        }
        

    }
    public void Undo()
    {
        if (states.Count > 1)
        {
            GameState previousSnapshot = states[states.Count - 1];
            states.RemoveAt(states.Count - 1);
            RestoreGameState(previousSnapshot);
        }
    }
    private void RestoreGameState(GameState state)
    {
        player.transform.position = state.playerPos;
        pScript.moveDir = state.pMoveDir;
        pScript.faceDir = state.pFaceDir;

        GameObject[] pushables = GameObject.FindGameObjectsWithTag("Pushable");
        GameObject[] sunks = GameObject.FindGameObjectsWithTag("New Ground");

        foreach (GameObject obj in pushables)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in sunks)
        {
            Destroy(obj);
        }

        foreach (Vector3 pos in state.boxes)
        {
            Instantiate(box, pos, Quaternion.identity);
        }
        foreach (Vector3 pos in state.metalBoxes)
        {
            Instantiate(metalBox, pos, Quaternion.identity);
        }
        foreach (Vector3 pos in state.frozenBoxes)
        {
            Instantiate(frozenBox, pos, Quaternion.identity);
        }
        foreach (Vector3 pos in state.noTongueBoxes)
        {
            Instantiate(noTongueBox, pos, Quaternion.identity);
        }
        foreach (Vector3 pos in state.sunkBoxes)
        {
            Instantiate(sunkBox, pos, Quaternion.identity);
        }
        foreach (Vector3 pos in state.sunkMetalBoxes)
        {
            Instantiate(sunkMetalBox, pos, Quaternion.identity);
        }
        foreach (Vector3 pos in state.sunkFrozenBoxes)
        {
            Instantiate(sunkFrozenBox, pos, Quaternion.identity);
        }
        foreach (Vector3 pos in state.sunkNoTongueBoxes)
        {
            Instantiate(sunkNoTongueBox, pos, Quaternion.identity);
        }
        foreach (Vector3 pos in state.crackedGround)
        {
            Instantiate(cracked, pos, Quaternion.identity);
        }
    }
}
public class GameState
{
    public Vector3 playerPos;
    public Vector3 pMoveDir;
    public string pFaceDir;
    public List<Vector3> boxes = new List<Vector3>();
    public List<Vector3> sunkBoxes = new List<Vector3>();
    public List<Vector3> frozenBoxes = new List<Vector3>();
    public List<Vector3> sunkFrozenBoxes = new List<Vector3>();
    public List<Vector3> metalBoxes = new List<Vector3>();
    public List<Vector3> sunkMetalBoxes = new List<Vector3>();
    public List<Vector3> noTongueBoxes = new List<Vector3>();
    public List<Vector3> sunkNoTongueBoxes = new List<Vector3>();
    public List<Vector3> crackedGround = new List<Vector3>();
}

