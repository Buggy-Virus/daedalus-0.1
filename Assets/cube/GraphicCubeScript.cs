using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicCubeScript : MonoBehaviour
{
    // ================================================ CONFIGUREABLE VARIABLES
    public float moveSpeed = 10f;

    // ================================================ PUBLIC ATTRIBUTES
	public GameObject cube;
    public CubeScript cubeScript;
    public Transform graphicObject_transform;
    public Collider graphicObject_collider;

    // ================================================ FETCHED REFERENCES
    MouseControls controlScript;

    // ================================================ GLOBAL VARIABLES
    // input variables
    bool mouseDown = false;

    // movement variables
    public Vector3 goingTo;

    // ================================================ Call Actions

    public void callTargetedACtion(bool hit, RaycastHit hitInfo) {
        if (hit) {
            if (Input.GetMouseButtonDown(0)) {
                mouseDown = true;
            }

            if (Input.GetMouseButtonUp(0) && mouseDown) {
                ResolveActionsScript.resolveTargetedAction(ref controlScript.selectedObject, ref cube, controlScript.waitingAction, ref cubeScript.gameEnv);
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
        controlScript = GameObject.Find("Controls").GetComponent<MouseControls>();
        graphicObject_transform = gameObject.transform.GetChild(0);
        graphicObject_collider = graphicObject_transform.GetComponent<Collider>();
    }

    void Update()
    {
        inputControls();
        move();
    }
}
