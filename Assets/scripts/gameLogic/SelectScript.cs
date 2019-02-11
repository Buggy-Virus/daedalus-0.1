using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScript : MonoBehaviour
{
	MouseControls controlScript;
	TokenPrefabScript tokenPrefabScript;
    Collider objectCollider;

    bool clicked;

    // Start is called before the first frame update
    void Start() {
    	controlScript = GameObject.Find("Controls").GetComponent<MouseControls>();
    	tokenPrefabScript = gameObject.GetComponent<TokenPrefabScript>();
        objectCollider = gameObject.transform.GetChild(0).GetComponent<Collider>();
        clicked = false;        
    }

    // Update is called once per frame
    void Update() {
    	if (controlScript.controlMode == 3) {
    		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
 
            if (objectCollider.Raycast(ray, out hitInfo, Mathf.Infinity)) {
                if (Input.GetMouseButtonDown(0)) {
                	clicked = true;
                }

                if (Input.GetMouseButtonUp(0) && clicked) {
                    controlScript.selectedObject = gameObject;
                    controlScript.selectedToken = tokenPrefabScript.token;
                    Debug.Log(controlScript.selectedObject.transform.name);
                    clicked = false;
                }
            }
        }         
    }
}
