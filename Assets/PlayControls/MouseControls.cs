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
    public float rotation;
    public bool deleteMode = false;
    public string shapeType = "stone";
    public string wallType = "stone";
    public string tokenType = "goblin";

    // ======================================================================= Prefabs
    public GameObject dragPrefab;
    public GameObject tokenPrefab;
    public GameObject shapePrefab;
    public GameObject wallPrefab;

    // ======================================================================= Public Variables
    // Used for play controls
    public int SELECT_MODE = 0;
    public int INPUT_MODE = 1;
    public GameObject selectedObject;
    public TokenScript selectedTokenScript;
    public ShapeScript selectedShapeScript;
    public Action waitingAction;

    // ======================================================================= Fetched References
    public GameObject main_camera;
    MapScript mapScript;
    GameEnvScript gameEnvScript;
    GameEnv gameEnv;
    GameObject editorPanel;
    Dropdown layerDropdown;
    Dropdown editorModeDropdown;
    InputField tokenTypeInput;
    InputField shapeTypeInput;

    // ======================================================================= Camera Controls Globals
    public int panThreshold = 15; // distance from edge scrolling starts
    public int panSpeed = 5;
    int screenWidth;
    int screenHeight;


    // ======================================================================= Global Variables
    // Used for editor controls
    public const int SHAPE_MODE = 0;
    public const int TOKEN_MODE = 1;
    public const int WALL_MODE = 2;

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
            destroyCursor();
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
            case SHAPE_MODE:
                addCursor(gameEnvScript.gameEnv.shapeTemplates[shapeType].graphicObjectPrefab, "cursorCube");
                break;
            case TOKEN_MODE:
                addCursor(gameEnvScript.gameEnv.tokenTemplates[tokenType].graphicObjectPrefab, "cursorToken");
                break;
            case WALL_MODE:
                addCursor(gameEnvScript.gameEnv.wallTemplates[wallType].graphicObjectPrefab, "cursorWall");
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
        if (gameEnvScript.gameEnv.shapeTemplates.ContainsKey(shapeTypeInput.text)) {
            tokenType = shapeTypeInput.text;
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
                    shapeMode(currentCoord, currentIndex);
                    placeCursor(currentCoord, 0);
                    break;
                case 1:
                    tokenMode(currentCoord, currentIndex);
                    placeCursor(currentCoord, 0);
                    break;
                case 2: wallMode(hitInfo.point, currentCoord);
                    break;
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

    void placeCursor(Vector3 currentCoord, float rotation) {
        if (activeCursor) {
            if (!cursorObjectRenderer.enabled) {
                cursorObjectRenderer.enabled = true;
            }
            cursorObject.transform.position = currentCoord + new Vector3(0.5f, 0.5f, 0.5f);
            cursorObject.transform.eulerAngles = new Vector3(0, rotation, 0);
        }
    }

    void addCursor (GameObject curCursorObject, string cursorName) {
        cursorObject = Instantiate(curCursorObject, gameObject.transform);
        cursorObjectRenderer = cursorObject.GetComponent<MeshRenderer>();
        cursorObjectRenderer.material.color = cursorTint;
        cursorObject.name = cursorName;
        activeCursor = true;
    }

    // ================================== Cube Mode
    void shapeMode(Vector3 curPosition, Index curIndex) {
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
                            GameUtils.createAndPlaceShape(shapePrefab, gameEnv.shapeTemplates[shapeType], ref gameEnv, curPosition);
	                    }
	                }
	                Destroy(dragObject);
	            } else if (Utils.indexEqual(mouseDownIndex, curIndex)) {
	            	Debug.Log("Placing Cube");
                    GameUtils.createAndPlaceShape(shapePrefab, gameEnv.shapeTemplates[shapeType], ref gameEnv, curPosition);
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
    	if (gameCoord.shape != null) {
            Destroy(gameCoord.shape);
        }
    	gameCoord.shape = null;
    }

    // ================================== Token Mode
    void tokenMode(Vector3 curPosition, Index curIndex) {
        

        if (!deleteMode) {
	        if (Input.GetMouseButtonDown(0)) {
	            cursorObjectRenderer.material.color = cursorClickHighlight;
	        }

	        if (Input.GetMouseButtonUp(0)) {
	            GameUtils.createAndPlaceToken(tokenPrefab, gameEnv.tokenTemplates[tokenType], ref gameEnv, curPosition);
	        }

	        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
	            cursorObjectRenderer.material.color = normalColor;
	        }
        }
    }

    // ================================== Wall Mode
    void wallMode(Vector3 mousePosition, Vector3 currentCoord) {
        Vector3 wallPosition;
        wallPosition.y = currentCoord.y;
        float wall_rotation = 0;
        if (Math.Abs(Math.Round(mousePosition.x) - mousePosition.x) < Math.Abs(Math.Round(mousePosition.z) - mousePosition.z)) {
            wallPosition.x = (float)(Math.Round(mousePosition.x)) - 0.5f;
            wallPosition.z = currentCoord.z;
            wall_rotation = 90;
        } else {
            wallPosition.x = currentCoord.x;
            wallPosition.z = (float)(Math.Round(mousePosition.z)) - 0.5f;
        }

        placeCursor(wallPosition, wall_rotation);

        if (!deleteMode) {
	        if (Input.GetMouseButtonDown(0)) {
	            cursorObjectRenderer.material.color = cursorClickHighlight;
	        }

	        if (Input.GetMouseButtonUp(0)) {
	            GameUtils.createAndPlaceWall(wallPrefab, gameEnv.wallTemplates[wallType], ref gameEnv, wallPosition, wall_rotation);
	        }

	        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
	            cursorObjectRenderer.material.color = normalColor;
	        }
        }
    }

    // ======================================================================= Play Mode
    void playControls() {

    }

    public void waitForActionInput(Action action) {
        Debug.Log("waitForActionInput Called");
        playMode = 1;
        waitingAction = action;
    }

    public void gotGoodInput() {
        Debug.Log("Got An Input");
        waitingAction = null;
        playMode = 0;
    }

    public void gotBadInput() {
        Debug.Log("Not a valid input");
    }

    public void cancelInput() {
        waitingAction = null;
        playMode = 0;
    }

    // ======================================================================= Camera Controls
    void screenPan() {
        if (Input.mousePosition.x > screenWidth - panThreshold) {
            Vector3 newPosition = main_camera.transform.position + new Vector3(panSpeed * Time.deltaTime, 0, -1 * panSpeed * Time.deltaTime); // move on +X axis
            if (newPosition.x > mapScript.sizeX) {
                newPosition.x = mapScript.sizeX;
            }
            if (newPosition.z < 0) {
                newPosition.z = 0;
            }
            main_camera.transform.position = newPosition;
        }
        if (Input.mousePosition.x < 0 + panThreshold) {
            Vector3 newPosition = main_camera.transform.position + new Vector3(-1 * panSpeed * Time.deltaTime, 0, panSpeed * Time.deltaTime); // move on +X axis
            if (newPosition.x < 0) {
                newPosition.x = 0;
            }
            if (newPosition.z > mapScript.sizeZ) {
                newPosition.z = mapScript.sizeZ;
            }
            main_camera.transform.position = newPosition;
        }
        if (Input.mousePosition.y > screenHeight - panThreshold) {
            Vector3 newPosition = main_camera.transform.position + new Vector3(panSpeed * Time.deltaTime, 0, panSpeed * Time.deltaTime); // move on +Z axis
            if (newPosition.x > mapScript.sizeX) {
                newPosition.x = mapScript.sizeX;
            }
            if (newPosition.z > mapScript.sizeZ) {
                newPosition.z = mapScript.sizeZ;
            }
            main_camera.transform.position = newPosition;
        }
        if (Input.mousePosition.y < 0 + panThreshold) {
            Vector3 newPosition = main_camera.transform.position + new Vector3(-1 * panSpeed * Time.deltaTime, 0, -1 * panSpeed * Time.deltaTime); // move on -Z axis
            if (newPosition.x < 0) {
                newPosition.x = 0;
            }
            if (newPosition.z < 0) {
                newPosition.z = 0;
            }
            main_camera.transform.position = newPosition;
        }
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
        shapeTypeInput = GameObject.Find("Cube_InputField").GetComponent<InputField>();
        tokenTypeInput = GameObject.Find("Token_InputField").GetComponent<InputField>();
        shapeTypeInput.text = shapeType;
        tokenTypeInput.text = tokenType;
    }

    // ======================================================================= START/UPDATE
    void Start() {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        mapScript = GameObject.Find("Map").GetComponent<MapScript>(); // This can be assigned directly
        gameEnvScript = GameObject.Find("GameLogic").GetComponent<GameEnvScript>(); // This can be assigned directly
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
        // screenPan();
    }
}

