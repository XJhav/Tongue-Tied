using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneState
{
    public List<SerializedObjectState> objectStates = new List<SerializedObjectState>();
}

[System.Serializable]
public class SerializedObjectState
{
    public string name;
    public string tag;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    // Add more properties as needed
}

public class SceneUndoManager : MonoBehaviour
{
    private Stack<SceneState> undoStack = new Stack<SceneState>();

    // Record the current state of the scene
    public void RecordUndoState()
    {
        SceneState sceneState = SerializeScene();
        undoStack.Push(sceneState);
    }

    // Undo to the previous state
    public void Undo()
    {
        if (undoStack.Count > 1)
        {
            SceneState previousState = undoStack.Pop();
            LoadSceneState(previousState);
        }
    }

    // Serialize the current scene state
    private SceneState SerializeScene()
    {
        SceneState sceneState = new SceneState();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Pushable") || obj.CompareTag("Unpushable") || obj.CompareTag("New Ground") || obj.CompareTag("Player"))
            {
                SerializedObjectState objectState = new SerializedObjectState
                {
                    name = obj.name,
                    tag = obj.tag,
                    position = obj.transform.position,
                    rotation = obj.transform.rotation,
                    scale = obj.transform.localScale
                    // Serialize other properties as needed
                };

                sceneState.objectStates.Add(objectState);
            }
        }

        return sceneState;
    }

    // Load a scene state
    private void LoadSceneState(SceneState state)
    {
        // Destroy existing objects
        GameObject[] existingObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in existingObjects)
        {
            Destroy(obj);
        }

        // Instantiate objects based on the serialized state
        foreach (SerializedObjectState objectState in state.objectStates)
        {
            GameObject newObj = new GameObject(objectState.name);
            newObj.tag = objectState.tag;
            newObj.transform.position = objectState.position;
            newObj.transform.rotation = objectState.rotation;
            newObj.transform.localScale = objectState.scale;
            // Set other properties as needed
        }
    }
}
