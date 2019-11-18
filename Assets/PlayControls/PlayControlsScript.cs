using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControlsScript : MonoBehaviour
{
    public GameEnvScript gameEnvScript;
    GameEnv gameEnv;

    // ================================== mode variables
    public bool selectMode = true;

    // ================================== mouse handling
    GameObject mdObject;

    // ================================== selected variables
    public GameObject selectedObject;
    public TokenScript selectedTokenScript;
    public ShapeScript selectedShapeScript;

    // ================================== call action variables
    public Action waitingAction;

    // ==================================
    // ================================== 
    // ================================== Input Functions
    public void waitForActionInput(Action action) {
        Debug.Log("waitForActionInput Called");
        selectMode = false;
        waitingAction = action;
    }

    public void gotGoodInput() {
        Debug.Log("Got An Input");
        waitingAction = null;
        selectMode = true;
    }

    public void gotBadInput() {
        Debug.Log("Not a valid input");
    }

    public void cancelInput() {
        waitingAction = null;
        selectMode = true;
    }

    public void clearInputs() {
        if (selectedTokenScript != null) {
            selectedTokenScript.graphicTokenScript.hideButtons();
        }

        waitingAction = null;
        selectMode = true;

        selectedObject = null;
        selectedShapeScript = null;
        selectedTokenScript = null;

        mdObject = null;
    }

    void deselect() {
        selectedTokenScript.graphicTokenScript.hideButtons();
    }

    public void selectNew(GameObject selected, TokenScript tokenScript) {
        deselect();
        selectedObject = selected;
        selectedTokenScript = tokenScript;
        selectedTokenScript.graphicTokenScript.showButtons();
    }

    void selectRun(ref GameObject hitObject) {
        TokenScript tokenScript = hitObject.GetComponent<TokenScript>();
        if (tokenScript != null) {
            selectNew(hitObject, tokenScript);
        }
    }

    void callRelationalAction(ref GameObject target, int distance) {
        if (distance < waitingAction.minRange) {
            gotBadInput();
            gameEnv.console.ConsoleLog("Distance to target below minimum range");
        } else if (distance > waitingAction.maxRange) {
            gotBadInput();
            gameEnv.console.ConsoleLog("Distance to target above maximum range");
        } else if (!ResolveActionsScript.resolveRelationalConditions(selectedObject, target, waitingAction.call_conditions, gameEnv)) {
            gotBadInput();
            gameEnv.console.ConsoleLog("Failed call condition");
        } else {
            ResolveActionsScript.resolveRelationalAction(ref selectedObject, ref target, waitingAction, ref gameEnv);
            gotGoodInput();
        }
    }

    void callTargetedAction(ref GameObject target, int distance) {
        if (distance < waitingAction.minRange) {
            gotBadInput();
            gameEnv.console.ConsoleLog("Distance to target below minimum range");
        } else if (distance > waitingAction.maxRange) {
            gotBadInput();
            gameEnv.console.ConsoleLog("Distance to target above maximum range");
        } else if (!ResolveActionsScript.resolveTargetedConditions(selectedObject, target, waitingAction.call_conditions, gameEnv)) {
            gotBadInput();
            gameEnv.console.ConsoleLog("Failed call condition");
        } else {
            ResolveActionsScript.resolveTargetedAction(ref selectedObject, ref target, waitingAction, ref gameEnv);
            gotGoodInput();
        }
    }

    void inputRun(ref GameObject hitObject) {
        TokenScript tokenScript = hitObject.GetComponent<TokenScript>();
        ShapeScript shapeScript = hitObject.GetComponent<ShapeScript>();
        WallScript wallScript = hitObject.GetComponent<WallScript>();
        if (tokenScript != null && waitingAction.relational) {
            int distance = Utils.distance(tokenScript.index, selectedTokenScript.index);
            callRelationalAction(ref hitObject, distance);
        } else if (shapeScript != null && waitingAction.targeted) {
            int distance = Utils.distance(shapeScript.index, selectedTokenScript.index);
            callTargetedAction(ref hitObject, distance);
        } else if (wallScript != null && waitingAction.wallTargeted) {
            // TODO
        }
    }

    void mouseDown() {
        RaycastHit hitPoint;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        if(Physics.Raycast(ray, out hitPoint, Mathf.Infinity)) {
            mdObject = hitPoint.collider.gameObject;
        }
    }

    void mouseUp() {
        RaycastHit hitPoint;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        if(Physics.Raycast(ray, out hitPoint, Mathf.Infinity)) {
            GameObject hitObject = hitPoint.collider.gameObject;
            if (selectMode) {               
                selectRun(ref hitObject);
            } else {
                inputRun(ref hitObject);
            }
        }
        mdObject = null;
    }

    void start() {
        gameEnv = gameEnvScript.gameEnv;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            mouseDown();
        } else if (Input.GetMouseButtonUp(0)) {
            mouseUp();
        }
    }
}
