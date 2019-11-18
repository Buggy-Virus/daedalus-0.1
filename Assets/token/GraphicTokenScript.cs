using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicTokenScript : MonoBehaviour
{
    // ================================================ PREFABS
    public GameObject canvasPrefab;
    public GameObject moveButtonPrefab;
    public GameObject basicButtonPrefab;
    public GameObject advancedButtonPrefab;
    public GameObject miscButtonPrefab;
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
    public GameObject canvas;
    public Transform graphicObject_transform;
    public Collider graphicObject_collider;

    // ================================================ FETCHED REFERENCES
    public PlayControlsScript controlScript;

    // ================================================ GLOBAL VARIABLES
    // selection variables

    // button variables
    int num_buttons = 4;
    GameObject moveButton;
    GameObject basicButton;
    GameObject advancedButton;
    GameObject miscButton;

    // movement variables
    public Vector3 goingTo;

    // ================================================ BUTTONS
    void createButtons() {
        List<GameObject> buttonList = new List<GameObject>();
        moveButton = Instantiate(moveButtonPrefab, canvas.transform, false);
        moveButton.name = "MoveButton";
        moveButton.SetActive(false);
        basicButton = Instantiate(basicButtonPrefab, canvas.transform, false);
        basicButton.name = "BasicButton";
        basicButton.SetActive(false);
        advancedButton = Instantiate(advancedButtonPrefab, canvas.transform, false);
        advancedButton.name = "advancedButton";
        advancedButton.SetActive(false);
        miscButton = Instantiate(miscButtonPrefab, canvas.transform, false);
        miscButton.name = "miscButton";
        miscButton.SetActive(false);

        moveButton.GetComponent<Button>().onClick.AddListener(moveButtonOnClick);
        basicButton.GetComponent<Button>().onClick.AddListener(basicButtonOnClick);
        advancedButton.GetComponent<Button>().onClick.AddListener(advancedButtonOnClick);
        miscButton.GetComponent<Button>().onClick.AddListener(miscButtonOnClick);
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

    public void hideButtons() {
        moveButton.SetActive(false);
        basicButton.SetActive(false);
        advancedButton.SetActive(false);
        miscButton.SetActive(false);
    }

    // ================================================ Button Controls

    void moveButtonOnClick() {
        Debug.Log("Clicked Move Button");
        Debug.Log("Printing token name:" + token.name);

        createActionMenu(1);
    }

    void basicButtonOnClick() {
        Debug.Log("Clicked Basic Button");
        Debug.Log("Printing token name:" + token.name);

        createActionMenu(2);
    }

    void advancedButtonOnClick() {
        Debug.Log("Clicked Advanced Button");
        Debug.Log("Printing token name:" + token.name);

        createActionMenu(3);
    }

    void miscButtonOnClick() {
        Debug.Log("Clicked Misc Button");
        Debug.Log("Printing token name:" + token.name);

        createActionMenu(4);
    }

    void createActionMenu(int menu) {
        GameObject actionMenu = Instantiate(actionMenuPrefab, canvas.transform);
        Transform actionMenuContent = actionMenu.transform.Find("Viewport").Find("Content");
        foreach(KeyValuePair<string, Action> action in tokenScript.actions) {
            if (action.Value.actionType == menu) {
                GameObject actionButton = Instantiate(actionMenuButtonPrefab, actionMenuContent);
                actionButton.name = action.Key;
                Button actionButtonButton = actionButton.GetComponent<Button>();
                actionButtonButton.GetComponentInChildren<Text>().text = action.Key;
                actionButtonButton.onClick.AddListener(delegate{callAction(action.Value, ref tokenScript.gameEnv);});
            }  
        }
    }

    // ================================================ Call Actions
    public void callAction(Action action, ref GameEnv gameEnv) {
        Debug.Log("Call Action");
        if (!action.relational && !action.targeted) {
            Debug.Log("Calling Self Action");
            ResolveActionsScript.resolveAction(ref token, action, ref gameEnv);
        } else {
            Debug.Log("Waiting For Input");
            controlScript.waitForActionInput(action);
            hideButtons();
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
        createButtons();
    }

    void Update() {
        move();
    }
}
