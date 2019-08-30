using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using UnityEngine;

public class MouseControls : MonoBehaviour {

    // ======================================================================= Configurable Variables
    public bool editor = true;
    public int editorMode = 0;
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
    public GameObject selectedToken;
    public GameObject selectedObject;
    public bool waitingForPlayerInput = false;

    // ======================================================================= Fetched References
    MapScript mapScript;
    GameEnvScript gameEnvScript;
    GameEnv gameEnv;

    // ======================================================================= Global Variables
    // Used for editor controls
    public int CUBE_MODE = 0;
    public int TOKEN_MODE = 1;

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
    int lastActiveLayer = -1;
    int lastEditorMode = -1;
    string lastCubeType = "stone";
    string lastTokenType = "goblin";

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

    void updateActiveLayer() {
        if (lastActiveLayer != activeLayer) {
            activeLayerObject = GameObject.Find("layer" + activeLayer.ToString());
            activeLayerCollider = activeLayerObject.GetComponent<MeshCollider>();

            lastActiveLayer = activeLayer;
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

    void addCursor (Vector3 curPosition, GameObject curCursorObject, string cursorName) {
        cursorObject = Instantiate(curCursorObject, curPosition, Quaternion.identity, gameObject.transform);
        cursorObjectRenderer = cursorObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        cursorObjectRenderer.material.color = cursorTint;
        cursorObject.name = cursorName;
        activeCursor = true;
    }

    // ================================== Cube Mode
    void cubeMode(Vector3 curPosition, Index curIndex) {
        if (lastEditorMode != CUBE_MODE || lastCubeType != cubeType) {
            Debug.Log("Cube mode");
            destroyCursor();
            Debug.Log((string)cubeType);
            addCursor(curPosition, gameEnvScript.gameEnv.cubeTemplates[cubeType].graphicObjectPrefab, "cursorCube");
            lastEditorMode = editorMode;
            lastCubeType = cubeType;
        }

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
        if (lastEditorMode != TOKEN_MODE || lastTokenType != tokenType) {
            Debug.Log("token mode");
            destroyCursor();
            addCursor(curPosition, gameEnvScript.gameEnv.tokenTemplates[tokenType].graphicObjectPrefab, "cursorToken");
            lastEditorMode = editorMode;
            lastTokenType = tokenType;
        }

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
        }
    }

    // ======================================================================= Basic Run
    void runControls() {
        updateActiveLayer();
        if (editor) {
            editorControls();
        } else {
            playControls();
        }
        lastEditor = editor;
    }

    // ======================================================================= START/UPDATE
    void Start() {
        mapScript = GameObject.Find("Map").GetComponent<MapScript>();
        gameEnvScript = GameObject.Find("GameLogic").GetComponent<GameEnvScript>();
        gameEnv = gameEnvScript.gameEnv;
    }    

    void Update() {
        updateActiveLayer();
        runControls();
    }
}

