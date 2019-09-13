using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicWallScript : MonoBehaviour
{
    // ================================================ CONFIGUREABLE VARIABLES
    // public float moveSpeed = 10f;

    // ================================================ PUBLIC ATTRIBUTES
    public WallScript wallScript;
    public Transform graphicObject_transform;
    public Collider graphicObject_collider;

    // ================================================ FETCHED REFERENCES
    GameObject wall;
    MouseControls controlScript; // Maybe this can be added directly for every prefab

    // ================================================ GLOBAL VARIABLES
    bool setupGraphicObject = false;
    // input variables
    bool mouseDown = false;

    // movement variables
    // public Vector3 goingTo;

    // ================================================ Call Actions

    public void callTargetedACtion(bool hit, RaycastHit hitInfo) {
        if (hit) {
            if (Input.GetMouseButtonDown(0)) {
                mouseDown = true;
            }

            if (Input.GetMouseButtonUp(0) && mouseDown) {
                ResolveActionsScript.resolveTargetedAction(ref controlScript.selectedObject, ref wall, controlScript.waitingAction, ref wallScript.gameEnv);
                controlScript.gotGoodInput();
            }
        }
    }

    void inputControls() {
        if (!controlScript.editor && controlScript.playMode == 1 && controlScript.waitingAction.targeted) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool hit = graphicObject_collider.Raycast(ray, out hitInfo, Mathf.Infinity);

            callTargetedACtion(hit, hitInfo);

            if (Input.GetMouseButtonUp(0)) {
                mouseDown = false;
            }
        }
    }
    
    // // ================================================ Movement
    // public void moveTo(Vector3 endPos) {
    //     goingTo = endPos;
    // }

    // void move() {
    //     if (transform.position != goingTo) {
    //         transform.position = Vector3.MoveTowards(transform.position, goingTo, moveSpeed * Time.deltaTime);
    //     } 
    // }

    // ================================================ Setup Graphic Object
    void settingUpGraphicObject() {
        if (!setupGraphicObject) {
            try {
                graphicObject_transform = gameObject.transform.GetChild(0);
                graphicObject_collider = graphicObject_transform.GetComponent<Collider>();
            } catch { 
                Debug.Log("GraphicObject not added yet");
            }
        }
    }

    // ================================================ START/UPDATE
    void Start() {
        wall = gameObject;
        wallScript = gameObject.GetComponent<WallScript>();
        graphicObject_transform = gameObject.transform.GetChild(0);
        graphicObject_collider = graphicObject_transform.GetComponent<Collider>();
    }

    void Update()
    {
        inputControls();
        // move();
    }
}
