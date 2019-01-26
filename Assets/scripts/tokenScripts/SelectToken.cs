using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectToken : MonoBehaviour {

    MouseControls controlScript;
    Collider objectCollider;

    bool clicked;

	// Use this for initialization
	void Start () {
        controlScript = GameObject.Find("Controls").GetComponent<MouseControls>();
        objectCollider = gameObject.GetComponent<Collider>();
        clicked = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (controlScript.controlMode == 3) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (objectCollider.Raycast(ray, out hitInfo, Mathf.Infinity)) {
                if (Input.GetMouseButtonDown(0)) {
                	clicked = true;
                }

                if (Input.GetMouseButtonUp(0) && clicked) {
                    controlScript.selectedObject = gameObject.transform.parent.gameObject;
                    Debug.Log(controlScript.selectedObject.transform.name);
                }
            }
        } else {
        	if (Input.GetMouseButtonUp(0) && controlScript.selectedObject == gameObject.transform.parent.gameObject) {
                    controlScript.selectedObject = null;
                    Debug.Log("nothing selected");
                }
        }
        
    }
}
