using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicTokenScript : MonoBehaviour
{
    // ================================================ PREFABS
    public GameObject actionsButtonPrefab;
    public GameObject actionMenuPrefab;
    public GameObject actionMenuButtonPrefab;

    // ================================================ CONFIGUREABLE VARIABLES
    public float moveSpeed = 10f;
    public float button_radius = 65;
    public float button_start = 0.5F;
    public float button_interval = 0.25F;

    // ================================================ PUBLIC ATTRIBUTES
	public GameObject token;
    public TokenScript tokenScript;
    public Transform canvas_transfrom;
    public Transform graphicObject_transform;
    public Collider graphicObject_collider;

    // ================================================ FETCHED REFERENCES
    MouseControls controlScript;

    // ================================================ GLOBAL VARIABLES
    // selection variables
    bool mouseDown;

    // button variables
    int num_buttons = 4;
    GameObject moveButton;
    GameObject basicButton;
    GameObject advancedButton;
    GameObject miscButton;
    bool buttonsActive;
    bool buttonsActive_lf;

    // movement variables
    public Vector3 goingTo;

    // ================================================ SELECTION CONTROLS
    void selectToken(bool hit, RaycastHit hitInfo) {
        if (hit && controlScript.selectedObject != gameObject) {
            if (Input.GetMouseButtonDown(0)) {
                mouseDown = true;
            }

            if (Input.GetMouseButtonUp(0) && mouseDown) {
                controlScript.selectedObject = token;
                Debug.Log(controlScript.selectedObject.transform.name);
                mouseDown = false;
                showButtons();
            }
        }
    }

    void playMode() {
        if (!controlScript.editor && controlScript.playMode == 0 ) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool hit = graphicObject_collider.Raycast(ray, out hitInfo, Mathf.Infinity);

            selectToken(hit, hitInfo);

            if (Input.GetMouseButtonUp(0)) {
                mouseDown = false;
            }

            if (controlScript.selectedObject == token && !moveButton.activeSelf) {
                showButtons();
            }
        }

        if (!controlScript.editor && controlScript.playMode == 0 && controlScript.selectedObject == token && Input.GetKeyDown(KeyCode.Escape)) {
            // ADD SHORTCUT
            Debug.Log("Canceled Input");
            controlScript.gotGoodInput();
        }
    }

    // ================================================ BUTTONS
    void createButtons() {
        List<GameObject> buttonList = new List<GameObject>();
        for (int i = 0; i < num_buttons; i++) {
            GameObject newButton = Instantiate(actionsButtonPrefab, canvas_transfrom);
            newButton.SetActive(false);
            buttonList.Add(newButton);
        }

        moveButton = Instantiate(actionsButtonPrefab, canvas_transfrom);
        moveButton.name = "MoveButton";
        basicButton = Instantiate(actionsButtonPrefab, canvas_transfrom);
        basicButton.name = "BasicButton";
        advancedButton = Instantiate(actionsButtonPrefab, canvas_transfrom);
        advancedButton.name = "advancedButton";
        miscButton = Instantiate(actionsButtonPrefab, canvas_transfrom);
        miscButton.name = "miscButton";

        basicButton.GetComponent<Button>().onClick.AddListener(basicButtonOnClick);
    }

    void placeButton(GameObject button, Vector3 tokenPosition, int Incremement) {
        float button_radians = button_start + Incremement * button_interval;
        Vector3 button_pos = Utils.polarToCartesian(button_radius, button_radians);
        button_pos.x = tokenPosition.x - button_pos.x;
        button_pos.y = tokenPosition.y + button_pos.y;
        button.transform.position = button_pos;
    }

    public void showButtons() {
        Vector3 graphicObject_ui_position = Camera.main.WorldToScreenPoint(graphicObject_transform.position);
        placeButton(moveButton, graphicObject_ui_position, 0);
        placeButton(basicButton, graphicObject_ui_position, 1);
        placeButton(advancedButton, graphicObject_ui_position, 2);
        placeButton(miscButton, graphicObject_ui_position, 3);

        moveButton.SetActive(true);
        basicButton.SetActive(true);
        advancedButton.SetActive(true);
        miscButton.SetActive(true);
    }

    void hideButtons() {
        moveButton.SetActive(false);
        basicButton.SetActive(false);
        advancedButton.SetActive(false);
        miscButton.SetActive(false);
    }

    // ================================================ Button Controls

    void basicButtonOnClick() {
        Debug.Log("Clicked Basic Button");
        Debug.Log("Printing token name:" + token.name);

        createActionMenu();
    }

    void createActionMenu() {
        GameObject actionMenu = Instantiate(actionMenuPrefab, canvas_transfrom);
        Transform actionMenuContent = actionMenu.transform.Find("Viewport").Find("Content");
        Debug.Log(actionMenuContent);
        foreach(KeyValuePair<string, Action> action in tokenScript.actions) {
            Debug.Log("Here");
            GameObject actionButton = Instantiate(actionMenuButtonPrefab, actionMenuContent);
            actionButton.name = action.Key;
            Button actionButtonButton = actionButton.GetComponent<Button>();
            actionButtonButton.GetComponentInChildren<Text>().text = action.Key;
            actionButtonButton.onClick.AddListener(delegate{callAction(action.Value, ref tokenScript.gameEnv);});
        }
    }

    void destroyActionMenu() {

    }

    // ================================================ Call Actions

    public void callAction(Action action, ref GameEnv gameEnv) {
        if (!action.relational && !action.targeted) {
            ResolveActionsScript.resolveAction(ref token, action, ref gameEnv);
            controlScript.waitForActionInput(action);
        } else {
            controlScript.waitForActionInput(action);
            hideButtons();
        }
    }

    public void callRelationalAction(bool hit, RaycastHit hitInfo) {
        if (hit && (controlScript.selectedObject != token || controlScript.waitingAction.selfTargetable)) {
            if (Input.GetMouseButtonDown(0)) {
                mouseDown = true;
            }

            if (Input.GetMouseButtonUp(0) && mouseDown) {
                ResolveActionsScript.resolveRelationalAction(ref controlScript.selectedObject, ref token, controlScript.waitingAction, ref tokenScript.gameEnv);
                controlScript.gotGoodInput();
            }
        }
    }

    void inputControls() {
        if (!controlScript.editor && controlScript.playMode == 1 && controlScript.waitingAction.relational) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool hit = graphicObject_collider.Raycast(ray, out hitInfo, Mathf.Infinity);

            callRelationalAction(hit, hitInfo);

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
        canvas_transfrom = gameObject.transform.GetChild(1);
        mouseDown = false; 
        createButtons();
    }

    void Update() {
        playMode();
        move();
        inputControls();
    }
}
