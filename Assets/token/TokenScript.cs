using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenScript : MonoBehaviour
{
    // ================================================ PREFABS
    public GameObject actionsButtonPrefab;

    // ================================================ CONFIGUREABLE VARIABLES
    public int num_buttons = 3;
    public float button_radius = 500;
    public float button_start = 0.5F;
    public float button_interval = 0.25F;

    // ================================================ PUBLIC ATTRIBUTES
	public Token token;
    public Transform canvas_transfrom;
    public Transform graphicObject_transform;
    public Collider graphicObject_collider;

    // ================================================ FETCHED REFERENCES
    MouseControls controlScript;

    // ================================================ GLOBAL VARIABLES
    // selection variables
    bool clicked;

    // button variables
    List<GameObject> buttonList;
    bool buttonsActive;
    bool buttonsActive_lf;

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
        buttonList = new List<GameObject>();
        for (int i = 0; i < num_buttons; i++) {
            float button_radians = button_start + i * button_interval;
            Vector3 button_pos = Utils.polarToCartesian(button_radius, button_radians);
            button_pos.x = graphicObject_ui_position.x - button_pos.x;
            button_pos.y = graphicObject_ui_position.y + button_pos.y;
            GameObject newButton = Instantiate(actionsButtonPrefab, button_pos, Quaternion.identity, canvas_transfrom);
            buttonList.Add(newButton);
        }
    }

    void destroyButtons() {

    }

    void buttons() {
        if (buttonsActive && !buttonsActive_lf) {
            Debug.Log("Creating Buttons");
            createButtons();
        } else if (!buttonsActive && buttonsActive_lf) {
            Debug.Log("Destroying Buttons");
            destroyButtons();
        }
        buttonsActive_lf = buttonsActive;
    }

    // ================================================ START/UPDATE
    void Start() {
        controlScript = GameObject.Find("Controls").GetComponent<MouseControls>();
        graphicObject_transform = gameObject.transform.GetChild(0);
        graphicObject_collider = graphicObject_transform.GetComponent<Collider>();
        canvas_transfrom = gameObject.transform.GetChild(1);
        clicked = false; 
    }

    void Update() {
        controls();
        buttons();
    }
}
