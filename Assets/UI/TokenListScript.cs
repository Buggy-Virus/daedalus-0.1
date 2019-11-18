using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenListScript : MonoBehaviour
{
    public GameEnvScript gameEnvScript;
    GameEnv gameEnv;
    public ModeScript modeScript;
    float cubeSize;
    public PlayControlsScript playControlsScript;
    public TokenEditorScript tokenEditorScript;

    public GameObject content;
    public GameObject entryPrefab;
    int numEntries = 0;

    bool onBoard = false;
    bool persistent = true;
    bool scenario = true;

    public string dragToken;
    public bool md;

    // ================================== selection Variables
    public Color selectedColor;
    public Color defaultColor;
    string selected;
    GameObject selectedEntry;

    // ================================== cursor Variables
    bool activeCursor = false;
    Color normalColor;
    public Color cursorTint;
    public Color cursorClickHighlight;
    GameObject cursorObject;
    
    // ==================================
    // ================================== General
    // ==================================
    public void clearInput() {
        md = false;
        dragToken = null;
        activeCursor = false;
        Destroy(cursorObject);

        if (modeScript.mode == 2) {
            tokenEditorScript.addCursor();
        }
    }

    // ==================================
    // ================================== Cursor
    // ==================================
    public void deactivateCursor() {
        activeCursor = false;
        Destroy(cursorObject);
    }

    void placeCursor(Vector3 currentCoord) {
        cursorObject.transform.position = currentCoord + new Vector3(0.5f, 0.5f, 0.5f);
    }

    public void addCursor (string tokenName) {
        if (tokenEditorScript.activeCursor) {
            tokenEditorScript.deactivateCursor();
        }

        if (activeCursor) {
            Destroy(cursorObject);
        }
        cursorObject = Instantiate(gameEnv.tokenDict[tokenName].GetComponent<TokenScript>().graphicObjectPrefab, gameObject.transform);
        cursorObject.GetComponent<Collider>().enabled = false;
        cursorObject.GetComponent<MeshRenderer>().material.color = cursorTint;
        cursorObject.name = tokenName + "_cursor";
        activeCursor = true;
    }

    // ==================================
    // ================================== UI functions
    // ==================================
    public void refresh() {
        foreach (Transform child in content.transform) {
            GameObject.Destroy(child.gameObject);
        }

        numEntries = 0;

        foreach (KeyValuePair<string, GameObject> token in gameEnv.tokenDict) {
            TokenScript tokenScript = token.Value.GetComponent<TokenScript>();

            if (((onBoard && tokenScript.onMap) || !onBoard) && ((persistent && tokenScript.persistent) || (scenario && !tokenScript.persistent))) {
                AddEntry(token.Key);
            }
        }
    }

    System.Action deleteToken(string name) {
        return delegate() {
            GameUtils.deleteToken(gameEnv.tokenDict[name]);
            refresh();
        };
    }

    System.Action removeToken(string name) {
        return delegate() {
            GameUtils.deleteGraphicToken(gameEnv.tokenDict[name]);
            refresh();
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

            GameObject token = gameEnv.tokenDict[name];
            TokenScript tokenScript = token.GetComponent<TokenScript>();
            if (modeScript.mode == 0 && tokenScript.onMap) {
                playControlsScript.selectNew(token, tokenScript);
            }
        };
    }

    public void AddEntry(string name) {
        numEntries += 1;
        GameObject newEntry = GameObject.Instantiate(entryPrefab, content.transform);
        newEntry.transform.position = newEntry.transform.position + new Vector3(0, -40 * numEntries + 40, 0); 
        TokenEntryScript tokenEntryScript = newEntry.GetComponent<TokenEntryScript>();
        tokenEntryScript.tokenName = name;
        tokenEntryScript.tokenListScript = gameObject.GetComponent<TokenListScript>();
        newEntry.transform.GetChild(0).GetComponent<Text>().text = name;
        newEntry.name = name;

        newEntry.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(deleteToken(name)));
        newEntry.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(removeToken(name)));
        newEntry.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(selectTemplate(name, newEntry)));
    }

    // ==================================
    // ================================== Drag Onto Board
    // ==================================
    void placeOnShape(ShapeScript shapeScript) {
        Index shapeIndex = shapeScript.index;
        Index placeIndex = new Index(shapeIndex);
        placeIndex.z += 1;

        Vector3 placePos = new Vector3(placeIndex.x * cubeSize, placeIndex.y * cubeSize, placeIndex.z * cubeSize);
        placeCursor(placePos);

        if (Input.GetMouseButtonUp(0)) {
            GameUtils.placeGraphicToken(gameEnv.tokenDict[dragToken], placeIndex);
        }
    }

    void placeAtToken(TokenScript tokenScript) {
        Index tokenIndex = tokenScript.index;

        Vector3 placePos = new Vector3(tokenIndex.x * cubeSize, tokenIndex.y * cubeSize, tokenIndex.z * cubeSize);
        placeCursor(placePos);

        if (Input.GetMouseButtonUp(0)) {
            GameUtils.placeGraphicToken(gameEnv.tokenDict[dragToken], tokenIndex);
        }
    }

    void placeOnLayer(Vector3 curPos) {
        Vector3 placePos = new Vector3(Mathf.Floor(curPos.x), Mathf.Floor(curPos.y), Mathf.Floor(curPos.z));
        placeCursor(placePos);

        if (Input.GetMouseButtonUp(0)) {
            GameUtils.placeGraphicToken(gameEnv.tokenDict[dragToken], placePos);
        }
    }

    // ======================================================================= START/UPDATE
    void Start() {
        gameEnv = gameEnvScript.gameEnv;
        cubeSize = gameEnv.mapScript.cubeSize;
        refresh();
    }

    void Update() {
        if (md && (modeScript.mode == 0 || modeScript.mode == 2)) {
            RaycastHit hitPoint;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitPoint, Mathf.Infinity) && hitPoint.collider.gameObject.GetComponent<LayerScript>() != null) {
                GameObject hitObject = hitPoint.collider.gameObject;
                ShapeScript shapeScript = hitObject.GetComponent<ShapeScript>();
                TokenScript tokenScript = hitObject.GetComponent<TokenScript>();
                LayerScript layerScript = hitObject.GetComponent<LayerScript>();
                if (shapeScript != null) {
                    placeOnShape(shapeScript);
                } else if (tokenScript != null) {
                    placeAtToken(tokenScript);
                } else if (layerScript != null) {
                    placeOnLayer(hitPoint.point);
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                clearInput();
            }
        }
    }
}
