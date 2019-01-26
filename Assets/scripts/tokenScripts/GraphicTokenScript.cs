using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicTokenScript : MonoBehaviour {

	MouseControls controlScript;
    Collider objectCollider;
    GameObject canvas;
    Camera cam;

	public Token gameToken;

    public GameObject buttonPrefab;

    GameObject attackButton;
    GameObject moveButton;
    GameObject actButton;
    GameObject statButton;

    bool clicked = false;
    bool selected = false;

	void Start () {
        controlScript = GameObject.Find("Controls").GetComponent<MouseControls>();
        objectCollider = gameObject.transform.GetChild(0).GetComponent<Collider>();
        gameToken = controlScript.objectToToken[gameObject];
        canvas = GameObject.Find("Canvas");
        cam = Camera.main;
    }

    void Update () {
        if (controlScript.controlMode == 3) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (objectCollider.Raycast(ray, out hitInfo, Mathf.Infinity)) {
                if (Input.GetMouseButtonDown(0)) {
                    clicked = true;
                }

                if (Input.GetMouseButtonUp(0) && clicked) {
                    controlScript.selectedObject = gameObject;
                    controlScript.selectedToken = gameToken;
                    Debug.Log("Selected Object: " + gameObject.name);
                    Debug.Log("Selected Token: " + gameToken.name);
                }
            } else {
                if (Input.GetMouseButtonUp(0) && controlScript.selectedObject == gameObject) {
                    controlScript.selectedObject = null;
                    controlScript.selectedToken = null;
                    Debug.Log("nothing selected");
                }
            }
        }

        if (controlScript.selectedObject == gameObject && !selected) {
            displayButtons();
            selected = true;
        }

        if (controlScript.selectedObject != gameObject && selected) {
            Debug.Log("Here");
            destroyButtons();
            selected = false;
        }
    }

    void displayButtons() {
        attackButton = createButt(Mathf.PI / 6f, 100f);
        moveButton = createButt(Mathf.PI / 3f, 100f);
        actButton = createButt(0, 100f);
        statButton = createButt(Mathf.PI * 11 / 6, 100f);
    }

    Vector3 polarToCartesian(float t, float r) {
        float x = Mathf.Cos(t) * r;
        float y = Mathf.Sin(t) * r;
        return new Vector3 (x, y, 0);
    }

    GameObject createButt(float t, float r) {
        Vector3 offset = polarToCartesian(t, r);
        GameObject button = Instantiate(buttonPrefab);
        button.transform.SetParent(canvas.transform); 
        Vector3 objectCanvasPosition = cam.WorldToScreenPoint(gameObject.transform.GetChild(0).transform.position);
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 0);
        buttonRect.anchorMax = new Vector2(0, 0);
        buttonRect.anchoredPosition = objectCanvasPosition + offset;
        //button.transform.position = new Vector3(gameObject.transform.position.x + coords[0], gameObject.transform.position.y + coords[1], 0);
        return button;
    }

    void destroyButtons() {
        Destroy(attackButton);
        Destroy(moveButton);
        Destroy(actButton);
        Destroy(statButton);
    }
}
