using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MouseControls : MonoBehaviour {

    // ======================================================================= Configurable Variables
    public bool editor = true;
    public int editorMode = 0;
    public int playMode = 0;
    public int activeLayer = 0;
    public bool deleteMode = false;
    public string cubeType = "stone";
    public string tokenType = "goblin";

    // ======================================================================= Prefabs
    public GameObject dragPrefab;
    public GameObject tokenPrefab;
    public GameObject cubePrefab;

    // ======================================================================= Public Variables
    // Used for play controls
    public int SELECT_MODE = 0;
    public int INPUT_MODE = 1;
    public GameObject selectedToken;
    public GameObject selectedObject;
    public Action waitingAction;
    public bool waitingInputRelational = false;
    public bool waitingInputTargeted = false;

    // ======================================================================= Fetched References
    MapScript mapScript;
    GameEnvScript gameEnvScript;
    GameEnv gameEnv;
    GameObject editorPanel;
    Dropdown layerDropdown;
    Dropdown editorModeDropdown;
    InputField tokenTypeInput;
    InputField cubeTypeInput;


    // ======================================================================= Global Variables
    // Used for editor controls
    public const int GEOMETRY_MODE = 0;
    public const int TOKEN_MODE = 1;

    // ================================== mouse state
    bool mouseDown;
    Vector3 mouseDownPosition = new Vector3(0, 0, 0);
    public Index mouseDownIndex;
    public Index mouseUpIndex;
    
    // ================================== cursor stuff
    bool activeCursor = false;
    public Color cursorTint;
    public Color cursorClickHighlight;
    Color normalColor;
    GameObject cursorObject;
    MeshRenderer cursorObjectRenderer;
    
    bool activeDrag;
    GameObject dragObject;    

    // ================================== Active Layer
    public GameObject activeLayerObject;
    Collider activeLayerCollider;

    // ================================== Frame update Checkers
    bool lastEditor = true;
    int lastPlayMode = -1;

    // ======================================================================= UI Functions

    public void hamburgerButton() {
        
    }

    public void gearButton() {

    }

    public void overlayButton() {

    }
    
    public void editorButton() {
        if (editor) {
            editor = false;
            editorPanel.SetActive(false); 
        } else {
            editor = true;
            editorPanel.SetActive(true); 
            changeCursor();
        }
    }

    public void overWorldButton() {

    }

    void changeCursor() {
        destroyCursor();
        switch (editorMode) {
            case GEOMETRY_MODE:
                addCursor(gameEnvScript.gameEnv.cubeTemplates[cubeType].graphicObjectPrefab, "cursorCube");
                break;
            case TOKEN_MODE:
                addCursor(gameEnvScript.gameEnv.tokenTemplates[tokenType].graphicObjectPrefab, "cursorToken");
                break;
        }
    }

    public void changeEditorMode() {
        editorMode = Convert.ToInt32(editorModeDropdown.value);
        changeCursor();
    }

    public void changeTokenType() {
        if (gameEnvScript.gameEnv.tokenTemplates.ContainsKey(tokenTypeInput.text)) {
            tokenType = tokenTypeInput.text;
            changeCursor();
        } else {
            Debug.Log("Bad Token Type");
        }
    }

    public void changeCubeType() {
        if (gameEnvScript.gameEnv.cubeTemplates.ContainsKey(cubeTypeInput.text)) {
            tokenType = cubeTypeInput.text;
            changeCursor();
        } else {
            Debug.Log("Bad Cube Type");
        }
    }

    public void changeActiveLayer() {
        int layer = Convert.ToInt32(layerDropdown.value);
        activeLayer = layer;
        activeLayerObject = GameObject.Find("layer" + activeLayer.ToString());
        activeLayerCollider = activeLayerObject.GetComponent<MeshCollider>();
    }

    // ======================================================================= Editor Mode
    void editorControls() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (activeLayerCollider.Raycast(ray, out hitInfo, Mathf.Infinity)) {

            int x = Mathf.FloorToInt(hitInfo.point.x / mapScript.cubeSize);
            int y = Mathf.FloorToInt(hitInfo.point.y / mapScript.cubeSize);
            int z = Mathf.FloorToInt(hitInfo.point.z / mapScript.cubeSize);

            Index currentIndex = new Index(x, y, z);

            Vector3 currentCoord;
            currentCoord.x = x * mapScript.cubeSize;
            currentCoord.y = y * mapScript.cubeSize;
            currentCoord.z = z * mapScript.cubeSize;

            switch (editorMode) {
                case 0:
                    cubeMode(currentCoord, currentIndex);
                    break;
                case 1:
                    tokenMode(currentCoord, currentIndex);
                    break;
            }

            if (activeCursor) {
                if (!cursorObjectRenderer.enabled) {
                    cursorObjectRenderer.enabled = true;
                }
                cursorObject.transform.position = currentCoord;
            }

        } else if (!activeLayerCollider.Raycast(ray, out hitInfo, Mathf.Infinity) && activeCursor) {
            cursorObjectRenderer.enabled = false;
        }
    }

    // ================================== Cursor Handling Functions
    void destroyCursor() {
        try {
            Destroy(cursorObject);
        } catch { 
            Debug.Log("Error Destroying Cursor");
        }
    }

    void addCursor (GameObject curCursorObject, string cursorName) {
        cursorObject = Instantiate(curCursorObject, gameObject.transform);
        cursorObjectRenderer = cursorObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        cursorObjectRenderer.material.color = cursorTint;
        cursorObject.name = cursorName;
        activeCursor = true;
    }

    // ================================== Cube Mode
    void cubeMode(Vector3 curPosition, Index curIndex) {
        

        if (!deleteMode) {
	        if (mouseDown && curPosition != mouseDownPosition && !activeDrag) {
	            activeDrag = true;
	            Vector3 dragObjectOrigin = new Vector3(mouseDownPosition.x + mapScript.cubeSize / 2, mouseDownPosition.y + mapScript.cubeSize / 2, mouseDownPosition.z + mapScript.cubeSize / 2);
	            dragObject = Instantiate(dragPrefab, dragObjectOrigin, Quaternion.identity, gameObject.transform);
	            dragObject.name = "DragObject";
	        }

	        if (activeDrag) {
	            dragObject.transform.localScale = new Vector3(Mathf.Abs(mouseDownPosition.x - curPosition.x) + mapScript.cubeSize, dragObject.transform.localScale.y, Mathf.Abs(mouseDownPosition.z - curPosition.z) + mapScript.cubeSize);
	            dragObject.transform.position = new Vector3((mouseDownPosition.x + curPosition.x + mapScript.cubeSize) / 2, dragObject.transform.position.y, (mouseDownPosition.z + curPosition.z + mapScript.cubeSize) / 2);
	        }

	        if (Input.GetMouseButtonDown(0)) {
	            cursorObjectRenderer.material.color = cursorClickHighlight;
	            mouseDownPosition = curPosition;
	            mouseDownIndex = new Index(curIndex);
	            mouseDown = true;
	        }

	        if (Input.GetMouseButtonUp(0)) {
	            Index iterIndex;
	            if (activeDrag) {
	                int y = curIndex.y;
	                for (int x = (int) Mathf.Min(mouseDownPosition.x, curPosition.x); x <= (int) Mathf.Max(mouseDownPosition.x, curPosition.x); x++) {
	                    for (int z = (int)Mathf.Min(mouseDownPosition.z, curPosition.z); z <= (int)Mathf.Max(mouseDownPosition.z, curPosition.z); z++) {
	                        iterIndex = new Index(x, y, z);
                            GameUtils.createAndPlaceCube(cubePrefab, gameEnv.cubeTemplates[cubeType], ref gameEnv, iterIndex);
	                    }
	                }
	                Destroy(dragObject);
	            } else if (Utils.indexEqual(mouseDownIndex, curIndex)) {
	            	Debug.Log("Placing Cube");
                    GameUtils.createAndPlaceCube(cubePrefab, gameEnv.cubeTemplates[cubeType], ref gameEnv, curIndex);
	            }
	            
	            mouseDown = false;
	            activeDrag = false;
	        }

	        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
	            cursorObjectRenderer.material.color = normalColor;
	            if (activeDrag) {
	                Destroy(dragObject);
	                mouseDown = false;
	                activeDrag = false;
	            }
	        }
        } else {
        	if (mouseDown && curPosition != mouseDownPosition && !activeDrag) {
	            activeDrag = true;
	            Vector3 dragObjectOrigin = new Vector3(mouseDownPosition.x + mapScript.cubeSize / 2, mouseDownPosition.y + mapScript.cubeSize / 2, mouseDownPosition.z + mapScript.cubeSize / 2);
	            dragObject = Instantiate(dragPrefab, dragObjectOrigin, Quaternion.identity, gameObject.transform);
	            dragObject.name = "DragObject";
	        }

	        if (activeDrag) {
	            dragObject.transform.localScale = new Vector3(Mathf.Abs(mouseDownPosition.x - curPosition.x) + mapScript.cubeSize, dragObject.transform.localScale.y, Mathf.Abs(mouseDownPosition.z - curPosition.z) + mapScript.cubeSize);
	            dragObject.transform.position = new Vector3((mouseDownPosition.x + curPosition.x + mapScript.cubeSize) / 2, dragObject.transform.position.y, (mouseDownPosition.z + curPosition.z + mapScript.cubeSize) / 2);
	        }

	        if (Input.GetMouseButtonDown(0)) {
	            cursorObjectRenderer.material.color = cursorClickHighlight;
	            mouseDownPosition = curPosition;
	            mouseDownIndex = new Index(curIndex);
	            mouseDown = true;
	        }

	        if (Input.GetMouseButtonUp(0)) {
	            Index iterIndex;
	            if (activeDrag) {
	                int y = curIndex.y;
	                for (int x = (int) Mathf.Min(mouseDownPosition.x, curPosition.x); x <= (int) Mathf.Max(mouseDownPosition.x, curPosition.x); x++) {
	                    for (int z = (int)Mathf.Min(mouseDownPosition.z, curPosition.z); z <= (int)Mathf.Max(mouseDownPosition.z, curPosition.z); z++) {
	                        iterIndex = new Index(x, y, z);
	                        deleteCube(iterIndex);
	                    }
	                }
	                Destroy(dragObject);
	            } else if (Utils.indexEqual(mouseDownIndex, curIndex)) {
	                deleteCube(curIndex);
	            }
	            
	            mouseDown = false;
	            activeDrag = false;
	        }

        }

    }

    void deleteCube(Index curIndex) {
    	GameCoord gameCoord = mapScript.gameBoard[curIndex.x, curIndex.y, curIndex.z];
    	if (gameCoord.cube != null) {
            Destroy(gameCoord.cube);
        }
    	gameCoord.cube = null;
    }

    // ================================== Token Mode
    void tokenMode(Vector3 curPosition, Index curIndex) {
        

        if (!deleteMode) {
	        if (Input.GetMouseButtonDown(0)) {
	            cursorObjectRenderer.material.color = cursorClickHighlight;
	        }

	        if (Input.GetMouseButtonUp(0)) {
	            GameUtils.createAndPlaceToken(tokenPrefab, gameEnv.tokenTemplates[tokenType], ref gameEnv, curIndex);
	        }

	        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
	            cursorObjectRenderer.material.color = normalColor;
	        }
        }
    }

    // ======================================================================= Play Mode
    void playControls() {
        if (lastEditor) {
            Debug.Log("select mode");
            destroyCursor();

            waitingAction = null;
            waitingInputRelational = false;
            waitingInputTargeted = false;
        }
    }

    public void gotGoodInput() {
        waitingAction = null;
        waitingInputRelational = false;
        waitingInputTargeted = false;
        playMode = 0;
    }

    public void gotBadInput() {
        Debug.Log("Not a valid input");
    }

    public void cancelInput() {
        waitingAction = null;
        waitingInputRelational = false;
        waitingInputTargeted = false;
        playMode = 0;
    }

    // ======================================================================= Basic Run
    void runControls() {
        if (editor) {
            editorControls();
        } else {
            playControls();
        }
    }

    void populateLayerDropDown() {
        layerDropdown = GameObject.Find("Layer_Dropdown").GetComponent<Dropdown>();

        List<string> options = new List<string>();
        for (int i = 0; i < mapScript.sizeY; i++) {
            options.Add(i.ToString());
        }
        layerDropdown.AddOptions(options);
    }

    void populateTypeInputs() {
        cubeTypeInput = GameObject.Find("Cube_InputField").GetComponent<InputField>();
        tokenTypeInput = GameObject.Find("Token_InputField").GetComponent<InputField>();
        cubeTypeInput.text = cubeType;
        tokenTypeInput.text = tokenType;
    }

    // ======================================================================= START/UPDATE
    void Start() {
        mapScript = GameObject.Find("Map").GetComponent<MapScript>();
        gameEnvScript = GameObject.Find("GameLogic").GetComponent<GameEnvScript>();
        gameEnv = gameEnvScript.gameEnv;
        editorPanel = GameObject.Find("Editor_Panel");
        editorModeDropdown = GameObject.Find("Mode_Dropdown").GetComponent<Dropdown>();
        populateLayerDropDown();
        populateTypeInputs();
        changeActiveLayer();
        changeCursor();
    }    

    void Update() {
        runControls();
    }
}

