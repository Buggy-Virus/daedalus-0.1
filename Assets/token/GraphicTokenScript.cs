using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicTokenScript : MonoBehaviour
{
    // ================================================ PREFABS
    public GameObject actionsButtonPrefab;

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
    bool clicked;

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

    // ================================================ CONTROLS
    void selectToken(bool hit, RaycastHit hitInfo) {
        if (hit && controlScript.selectedObject != gameObject) {
            if (Input.GetMouseButtonDown(0)) {
                clicked = true;
            }

            if (Input.GetMouseButtonUp(0) && clicked) {
                controlScript.selectedObject = gameObject;
                controlScript.selectedToken = token;
                Debug.Log(controlScript.selectedObject.transform.name);
                clicked = false;
                buttonsActive = false;
            }
        }
    }

    void showButtons(bool hit, RaycastHit hitInfo) {
        if (controlScript.selectedObject == gameObject && hit && !buttonsActive) {
            if (Input.GetMouseButtonDown(0)) {
                clicked = true;
            }

            if (Input.GetMouseButtonUp(0) && clicked) {
                buttonsActive = true;
            }
        }
    }

    void controls() {
        if (controlScript.controlMode == controlScript.SELECT_MODE) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool hit = graphicObject_collider.Raycast(ray, out hitInfo, Mathf.Infinity);

            selectToken(hit, hitInfo);

            showButtons(hit, hitInfo);

            if (Input.GetMouseButtonUp(0)) {
                clicked = false;
            }
        }
    }

    // ================================================ BUTTONS
    void createButtons() {
        Vector3 graphicObject_ui_position = Camera.main.WorldToScreenPoint(graphicObject_transform.position);
        List<GameObject> buttonList = new List<GameObject>();
        for (int i = 0; i < num_buttons; i++) {
            float button_radians = button_start + i * button_interval;
            Vector3 button_pos = Utils.polarToCartesian(button_radius, button_radians);
            button_pos.x = graphicObject_ui_position.x - button_pos.x;
            button_pos.y = graphicObject_ui_position.y + button_pos.y;
            GameObject newButton = Instantiate(actionsButtonPrefab, graphicObject_ui_position, Quaternion.identity, canvas_transfrom);
            newButton.GetComponent<RectTransform>().anchoredPosition = graphicObject_ui_position;
            newButton.SetActive(false);
            buttonList.Add(newButton);
        }

        moveButton = buttonList[0];
        moveButton.name = "MoveButton";
        basicButton = buttonList[1];
        basicButton.name = "BasicButton";
        advancedButton = buttonList[2];
        advancedButton.name = "advancedButton";
        miscButton = buttonList[3];
        miscButton.name = "miscButton";

        basicButton.GetComponent<Button>().onClick.AddListener(basicButtonOnClick);
    }

    void basicButtonOnClick() {
        Debug.Log("Clicked Basic Button");
        Debug.Log("Printing token name:" + token.name);
    }

    void showButtons() {
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

    void buttons() {
        if (buttonsActive && !buttonsActive_lf) {
            Debug.Log("Showing Buttons");
            showButtons();
        } else if (!buttonsActive && buttonsActive_lf) {
            Debug.Log("Hiding Buttons");
            hideButtons();
        }
        buttonsActive_lf = buttonsActive;
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
        canvas_transfrom = gameObject.transform.GetChild(1);
        clicked = false; 
        createButtons();
    }

    void Update() {
        controls();
        buttons();
        move();
    }
}
