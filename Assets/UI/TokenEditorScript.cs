using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TokenEditorScript : MonoBehaviour {

    // ================================== Prefabs
    public GameObject entryPrefab;
    public GameObject tokenPrefab;
    public Color selectedColor;
    public Color defaultColor;

    // ================================== UI elements
    public Dropdown layerDropdown;
    public InputField typeFilter;
    public InputField nameFilter;
    public GameObject content;

    // ================================== Scripts
    public GameEnvScript gameEnvScript;
    GameEnv gameEnv;
    public TokenListScript tokenEditorScript;

    // ================================== State variables
    int numEntries = 0;
    bool deleteMode;
    bool removeMode;
    public string selected = "";
    GameObject selectedEntry;

    // ================================== mouse state Variables
    bool md;
    Vector3 mdPos;

    // ================================== cursor Variables
    public bool activeCursor = false;
    Color normalColor;
    public Color cursorTint;
    public Color cursorClickHighlight;
    GameObject cursorObject;
    MeshRenderer cursorObjectRenderer;

    // ==================================
    // ================================== UI functions
    // ==================================
    public void changeActiveLayer() {
        gameEnv.mapScript.UpdateActiveLayer(layerDropdown.value);
    }

    System.Action addToken(string name) {
        return delegate() {
            GameUtils.quickCreateToken(tokenPrefab, gameEnv.tokenTemplates[name], ref gameEnv);
            tokenEditorScript.refresh();
        };
    }

    System.Action selectTemplate(string name, GameObject templateEntry) {
        return delegate() {
            if (selectedEntry != null) {
                selectedEntry.transform.GetComponent<Image>().color = defaultColor;
            }
            selectedEntry = templateEntry;
            templateEntry.transform.GetComponent<Image>().color = selectedColor;
            selected = name;

            addCursor(gameEnv.tokenTemplates[name].graphicObjectPrefab, name + "_cursor");
        };
    }

    void toggleDeleteMode() {
        if (deleteMode) {
            deleteMode = false;
        } else {
            deleteMode = true;
            removeMode = false;
        }
    }

    void toggleRemoveMode() {
        if (removeMode) {
            removeMode = false;
        } else {
            removeMode = true;
            deleteMode = false;
        }
    }

    // ==================================
    // ================================== Cursor
    // ==================================
    public void deactivateCursor() {
        if (activeCursor) {
            activeCursor = false;
            Destroy(cursorObject);
        }
    }

    void placeCursor(Vector3 currentCoord) {
        if (activeCursor) {
            if (!cursorObjectRenderer.enabled) {
                cursorObjectRenderer.enabled = true;
            }
            cursorObject.transform.position = currentCoord + new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    void addCursor (GameObject curCursorObject, string cursorName) {
        if (activeCursor) {
            Destroy(cursorObject);
        }
        cursorObject = Instantiate(curCursorObject, gameObject.transform);
        cursorObject.GetComponent<Collider>().enabled = false;
        cursorObjectRenderer = cursorObject.GetComponent<MeshRenderer>();
        cursorObjectRenderer.material.color = cursorTint;
        cursorObject.name = cursorName;
        activeCursor = true;
    }

    public void addCursor() {
        if (activeCursor) {
            Destroy(cursorObject);
        }

        if (selected != "") {
            cursorObject = Instantiate(gameEnv.tokenTemplates[selected].graphicObjectPrefab, gameObject.transform);
            cursorObject.GetComponent<Collider>().enabled = false;
            cursorObjectRenderer = cursorObject.GetComponent<MeshRenderer>();
            cursorObjectRenderer.material.color = cursorTint;
            cursorObject.name = selected + "_cursor";
            activeCursor = true;
        }
    }

    // ==================================
    // ================================== Mouse Handling
    // ==================================
    void startMouseDown(Vector3 curPos) {
        cursorObjectRenderer.material.color = cursorClickHighlight;
        md = true;
        mdPos = curPos;
    }

    public void clearInput() {
        if (cursorObject != null) {
            cursorObjectRenderer.material.color = normalColor;
        }
        md = false;
    }

    public void clearSelect() {
        selected = null;
        if (selectedEntry != null) {
            selectedEntry.transform.GetComponent<Image>().color = defaultColor;
            selectedEntry = null;
        }

        if (cursorObject != null) {
            Destroy(cursorObject);
        }
    }
    // ==================================
    // ================================== Add and Delete
    // ==================================
    void mouseUp(Vector3 curPos) {
        if (deleteMode) {
            GameUtils.deleteTokensAtPos(curPos, ref gameEnv.mapScript);
            tokenEditorScript.refresh();
        } else if (removeMode) {
            GameUtils.removeTokensAtPos(curPos, ref gameEnv.mapScript);
        } else {
            GameUtils.createAndPlaceToken(tokenPrefab, gameEnv.tokenTemplates[selected], ref gameEnv, curPos);
            tokenEditorScript.refresh();
        }
    }

    // ======================================================================= START/UPDATE
    void Start() {
        gameEnv = gameEnvScript.gameEnv;

        foreach (KeyValuePair<string, TokenTemplate> template in gameEnv.tokenTemplates) {
            numEntries += 1;
            GameObject newEntry = GameObject.Instantiate(entryPrefab, content.transform);
            newEntry.transform.position = newEntry.transform.position + new Vector3(0, -40 * numEntries + 40, 0); 
            newEntry.transform.GetChild(0).GetComponent<Text>().text = template.Key;
            newEntry.name = template.Key;

            newEntry.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(addToken(template.Key)));
            newEntry.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(selectTemplate(template.Key, newEntry)));
        }

        changeActiveLayer();
    }

    void Update() {
        RaycastHit hitPoint;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(selected != "" && Physics.Raycast(ray, out hitPoint, Mathf.Infinity) && hitPoint.collider.gameObject.GetComponent<LayerScript>() != null) {
            Vector3 curPos = new Vector3();
            curPos.x = Mathf.Floor(hitPoint.point.x);
            curPos.y = Mathf.Floor(hitPoint.point.y);
            curPos.z = Mathf.Floor(hitPoint.point.z);

            if (cursorObject != null) {
                placeCursor(curPos);
            }

            if (Input.GetMouseButtonDown(0)) {
                startMouseDown(curPos);
            } else if (Input.GetMouseButtonUp(0) && md && curPos == mdPos) {
                mouseUp(curPos);
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            clearInput();
        }
    }
}
