using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicShapeScript : MonoBehaviour
{
    // ================================================ CONFIGUREABLE VARIABLES
    public float moveSpeed = 10f;

    // ================================================ PUBLIC ATTRIBUTES
	public GameObject shape;
    public ShapeScript shapeScript;
    public Transform graphicObject_transform;
    public Collider graphicObject_collider;

    // ================================================ FETCHED REFERENCES
    MouseControls controlScript;

    // ================================================ GLOBAL VARIABLES
    bool setupGraphicObject = false;
    // input variables
    bool mouseDown = false;

    // movement variables
    public Vector3 goingTo;

    // ================================================ Call Actions

    public void callTargetedAction(bool hit, RaycastHit hitInfo) {
        if (hit) {
            if (Input.GetMouseButtonDown(0)) {
                mouseDown = true;
            }

            if (Input.GetMouseButtonUp(0) && mouseDown) {
                int distance = Utils.distance(shapeScript.index, controlScript.selectedTokenScript.index);
                if (controlScript.waitingAction.minRange < 0 || distance < controlScript.waitingAction.minRange) {
                    controlScript.gotBadInput();
                    Debug.Log("Distance to target below minimum range");
                } else if (controlScript.waitingAction.maxRange < 0 || distance > controlScript.waitingAction.maxRange) {
                    controlScript.gotBadInput();
                    Debug.Log("Distance to target above maximum range");
                } else if (!ResolveActionsScript.resolveTargetedConditions(controlScript.selectedObject, shape, controlScript.waitingAction.call_conditions, shapeScript.gameEnv)) {
                    controlScript.gotBadInput();
                    Debug.Log("Failed call condition");
                } else {
                    ResolveActionsScript.resolveTargetedAction(ref controlScript.selectedObject, ref shape, controlScript.waitingAction, ref shapeScript.gameEnv);
                    controlScript.gotGoodInput();
                }
            }
        }
    }

    void inputControls() {
        if (!controlScript.editor && controlScript.playMode == 1 && controlScript.waitingAction.targeted) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool hit = graphicObject_collider.Raycast(ray, out hitInfo, Mathf.Infinity);

            callTargetedAction(hit, hitInfo);

            if (Input.GetMouseButtonUp(0)) {
                mouseDown = false;
            }
        }
    }
    
    // ================================================ Movement
    public void moveTo(Vector3 endPos) {
        goingTo = endPos;
    }

    void move() {
        if (transform.position != goingTo) {
            transform.position = Vector3.MoveTowards(transform.position, goingTo, moveSpeed * Time.deltaTime);
        } 
    }

    // ================================================ START/UPDATE
    void Start() {
        shape = gameObject;
        controlScript = GameObject.Find("Controls").GetComponent<MouseControls>();
    }

    void Update()
    {
        inputControls();
        move();
    }
}
