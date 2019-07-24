using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using UnityEngine;

public class MouseControls : MonoBehaviour {

    public int CUBE_MODE = 0;
    public int TOKEN_MODE = 1;
    public int SELECT_MODE = 2;

	GameObject cubesObject;
    GameObject tokensObject;

    MapScript mapScript;
    GraphicScript graphicScript;
    GameEnvScript gameEnvScript;

    public GameObject activeLayerObject;
    Collider activeLayerCollider;

    public Index mouseDownIndex;
    public Index mouseUpIndex;
    Vector3 mouseDownPosition = new Vector3(0, 0, 0);
    bool mouseDown;

    GameObject cursorObject;
    MeshRenderer cursorObjectRenderer;
    bool activeCursor = false;

    public GameObject dragPrefab;
    GameObject dragObject;
    bool activeDrag;

    public Color cursorTint;
    public Color cursorClickHighlight;
    Color normalColor;

    public Token selectedToken;
    public GameObject selectedObject;

    public int activeLayer = 0;
    int lastActiveLayer = -1;

    public int controlMode = 0;
    int lastControlMode = -1;

    public bool deleteMode = false;

    public string cubeType = "testCube";
    string lastCubeType = "testCube";

    public string tokenType = "testToken";
    string lastTokenType = "testToken";

    bool usedDiagnolMove;

    void Start() {
    	cubeType = "testCube";
    	tokenType = "testToken";
    	Debug.Log(cubeType);
    	cubesObject = GameObject.Find("Cubes");
        tokensObject = GameObject.Find("Tokens");

        mapScript = GameObject.Find("Map").GetComponent<MapScript>();
        graphicScript = GameObject.Find("GameLogic").GetComponent<GraphicScript>();
        gameEnvScript = GameObject.Find("GameLogic").GetComponent<GameEnvScript>();

        updateActiveLayer();
    }

    // Update is called once per frame
    void Update() {
        updateActiveLayer();

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

            switch (controlMode) {
                case 0:
                    cubeMode(currentCoord, currentIndex);
                    break;
                case 1:
                    tokenMode(currentCoord, currentIndex);
                    break;
                case 2:
                    selectMode();
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

    void cubeMode(Vector3 curPosition, Index curIndex) {
        if (lastControlMode != CUBE_MODE || lastCubeType != cubeType) {
            Debug.Log("Cube mode");
            destroyCursor();
            Debug.Log((string)cubeType);
            addCursor(curPosition, graphicScript.cubePrefabs[cubeType], "cursorCube");
            lastControlMode = controlMode;
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
	                        placeCube(new Vector3(x, curPosition.y, z), iterIndex, cubeType);
	                    }
	                }
	                Destroy(dragObject);
	            } else if (Utils.indexEqual(mouseDownIndex, curIndex)) {
	            	Debug.Log("Placing Cube");
	                placeCube(curPosition, curIndex, cubeType);
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

    void placeCube(Vector3 curPosition, Index curIndex, string cubeType) {
        GameCoord gameCoord = mapScript.gameBoard[curIndex.x, curIndex.y, curIndex.z];

        Debug.Log(gameCoord.cube);

        if (gameCoord.cube != null) {
            Destroy(gameCoord.cube.graphicObject);
            Destroy(gameCoord.cube);
        }
        Debug.Log(cubeType);
        Debug.Log(gameEnvScript.cubeParameters[cubeType]);
        Cube newCube = gameEnvScript.createCube(gameEnvScript.cubeParameters[cubeType]);
        newCube.index = new Index(curIndex);

        gameCoord.cube = newCube;

        Debug.Log(graphicScript.cubePrefabs[cubeType]);
        GameObject graphicCube = Instantiate(graphicScript.cubePrefabs[cubeType], curPosition, Quaternion.identity, cubesObject.transform);

        newCube.graphicObject = graphicCube;
        graphicCube.GetComponent<CubeScript>().cube = newCube;
        graphicCube.name = newCube.uniqueIdentifier;
    }

    void deleteCube(Index curIndex) {
    	GameCoord gameCoord = mapScript.gameBoard[curIndex.x, curIndex.y, curIndex.z];
    	if (gameCoord.cube != null) {
            Destroy(gameCoord.cube.graphicObject);
        }
    	gameCoord.cube = null;
    }

    void tokenMode(Vector3 curPosition, Index curIndex) {
        if (lastControlMode != TOKEN_MODE || lastTokenType != tokenType) {
            Debug.Log("token mode");
            destroyCursor();
            addCursor(curPosition, graphicScript.tokenPrefabs[tokenType], "cursorToken");
            lastControlMode = controlMode;
            lastTokenType = tokenType;
        }

        if (!deleteMode) {
	        if (Input.GetMouseButtonDown(0)) {
	            cursorObjectRenderer.material.color = cursorClickHighlight;
	        }

	        if (Input.GetMouseButtonUp(0)) {
	            placeToken(curPosition, curIndex, tokenType);
	        }

	        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
	            cursorObjectRenderer.material.color = normalColor;
	        }
        }
    }

    void placeToken(Vector3 curPosition, Index curIndex, string tokenType) {
    	Debug.Log("Placed Token");
    	GameCoord gameCoord = mapScript.gameBoard[curIndex.x, curIndex.y, curIndex.z];

    	Token newToken = gameEnvScript.createToken(gameEnvScript.tokenParameters[tokenType]);
    	newToken.index = new Index(curIndex);
    	gameCoord.tokens.Add(newToken);

    	GameObject graphicToken = Instantiate(graphicScript.tokenPrefabs[tokenType], curPosition, Quaternion.identity, tokensObject.transform);
    	newToken.graphicObject = graphicToken;
        graphicToken.name = newToken.uniqueIdentifier;
    	TokenScript graphicTokenScript = graphicToken.GetComponent<TokenScript>();
        graphicTokenScript.token = newToken;
    }

    void destroyCursor () {
        try {
            Destroy(cursorObject);
        } catch { }
    }

    void addCursor (Vector3 curPosition, GameObject curCursorObject, string cursorName) {
        cursorObject = Instantiate(curCursorObject, curPosition, Quaternion.identity, gameObject.transform);
        cursorObjectRenderer = cursorObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        cursorObjectRenderer.material.color = cursorTint;
        cursorObject.name = cursorName;
        activeCursor = true;
    }

    void cursorlessControlMode () {
        try {
            Destroy(cursorObject);
        } catch { }
        activeCursor = false;
        lastControlMode = controlMode;
    }

    void selectMode() {
        if (lastControlMode != SELECT_MODE) {
            Debug.Log("select mode");
            cursorlessControlMode();
        }
    }
}

