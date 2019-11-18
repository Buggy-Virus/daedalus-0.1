using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorScript : MonoBehaviour
{
    public MapScript mapScript;
    public GameEnvScript gameEnvScript;
    GameEnv gameEnv;

    // ================================== modes and state Variables
    float size;
    float wallWidthMultiple = 0.1f;
    float wallWidth;

    int SHAPE_MODE = 0;
    int WALL_MODE = 1;
    int TILE_MODE = 2;

    int mode = 0;
    bool deleteMode = false;
    
    ShapeTemplate shapeType;
    WallTemplate wallType;
    TileTemplate tileType;
    public GameObject shapePrefab;
    public GameObject wallPrefab;
    public GameObject tilePrefab;

    // ================================== UI variables
    public Dropdown modeDropdown;
    public InputField shapeInput;
    public InputField wallInput;
    public InputField tileInput;
    public Toggle deleteToggle;
    public Dropdown layerDropdown;
    int activeLayer = 0;
    Collider layerCollider;

    // ================================== mouse state Variables
    bool md;
    Vector3 mdPos;

    // ================================== cursor Variables
    bool activeCursor = false;
    Color normalColor;
    public Color cursorTint;
    public Color cursorClickHighlight;
    GameObject cursorObject;
    MeshRenderer cursorObjectRenderer;
    
    // ================================== Drag Variables
    bool activeDrag;
    public GameObject dragObject;     

    // ==================================
    // ================================== UI
    // ==================================

    public void changeActiveLayer() {
        layerCollider = mapScript.UpdateActiveLayer(layerDropdown.value);
    }

    public void toggleDelete() {
        deleteMode = deleteToggle.isOn;
    }

    public void changeMode() {
        mode = modeDropdown.value;
        
        switch(mode) {
            case 0:
                shapeInput.gameObject.SetActive(true);
                wallInput.gameObject.SetActive(false);
                tileInput.gameObject.SetActive(false);
                break;
            case 1:
                shapeInput.gameObject.SetActive(false);
                wallInput.gameObject.SetActive(true);
                tileInput.gameObject.SetActive(false);
                break;
            case 2:
                shapeInput.gameObject.SetActive(false);
                wallInput.gameObject.SetActive(false);
                tileInput.gameObject.SetActive(true);
                break;
        }
    }

    public void changeShapeType() {
        string input = shapeInput.text;
        if (gameEnv.shapeDict.ContainsKey(input)) {
            shapeType = gameEnv.shapeTemplates[input];
            refreshCursor();
        } else {
            gameEnv.console.ConsoleLog("No shape template: \"" + input + "\"");
        }
    }

    public void changeWallType() {
        string input = wallInput.text;
        if (gameEnv.shapeDict.ContainsKey(input)) {
            wallType = gameEnv.wallTemplates[input];
            refreshCursor();
        } else {
            gameEnv.console.ConsoleLog("No wall template: \"" + input + "\"");
        }
    }

    public void changeTileType() {
        // TODO
    }

    // ==================================
    // ================================== Cursor
    // ==================================
    public void deactivateCurosr() {
        
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

    public void refreshCursor() {
        if (cursorObject != null) {
            Destroy(cursorObject);
        }

        switch(mode) {
            case 0:
                addCursor(shapeType.graphicObjectPrefab, "ShapeCursor");
                break;
            case 1:
                addCursor(wallType.graphicObjectPrefab, "ShapeCursor");
                break;
            case 2:
                // TODO
                break;
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

    void WallStartMouseDown(Vector3 curPos) {
        cursorObjectRenderer.material.color = cursorClickHighlight;
        md = true;
        mdPos = curPos;
    }

    public void clearInput() {
        if (cursorObject != null) {
            cursorObjectRenderer.material.color = normalColor;
        }
        md = false;
        dragObject.SetActive(false);
        activeDrag = false;
    }

    // ==================================
    // ================================== Drag Handling
    // ==================================

    void shapeStartDrag(Vector3 curPos) {
        activeDrag = true;
        dragObject.SetActive(true);
        dragObject.transform.localScale = new Vector3(size, size, size);
        dragObject.transform.position = curPos;
    }

    void wallStartDrag(Vector3 wallPos, float wallRotation) {
        activeDrag = true;
        dragObject.SetActive(true);
        if (wallRotation == 0) {
            dragObject.transform.localScale = new Vector3(size, size, wallWidth);
        } else {
            dragObject.transform.localScale = new Vector3(wallWidth, size, size);
        }
        dragObject.transform.position = wallPos;
    }

    void shapeActiveDrag(Vector3 curPos) {
        dragObject.transform.localScale = new Vector3(Mathf.Abs(mdPos.x - curPos.x) + size, size, Mathf.Abs(mdPos.z - curPos.z) + size);
        dragObject.transform.position = new Vector3((mdPos.x + curPos.x + size) / 2, curPos.y, (mdPos.z + curPos.z + size) / 2);
    }

    void wallActiveDrag(Vector3 wallPos) {
        if (wallPos.x - mdPos.x > wallPos.z - mdPos.z) {
            dragObject.transform.localScale = new Vector3(Mathf.Abs(mdPos.x - wallPos.x) + size, size, wallWidth);
            dragObject.transform.position = new Vector3((mdPos.x + wallPos.x + size) / 2, wallPos.y, wallPos.z);
        } else {
            dragObject.transform.localScale = new Vector3(wallWidth, size, Mathf.Abs(mdPos.z - wallPos.z) + size);
            dragObject.transform.position = new Vector3(wallPos.x, wallPos.y, (mdPos.z + wallPos.z + size) / 2);
        }
    }

    // ==================================
    // ================================== Shape Mode
    // ==================================
    void ShapePlace(Vector3 curPos) {
        if (activeDrag) {
            for (int x = (int)(Mathf.Min(mdPos.x, curPos.x) / size); x <= (int)(Mathf.Max(mdPos.x, curPos.x) / size); x++) {
                for (int z = (int)Mathf.Min(mdPos.z, curPos.z); z <= (int)Mathf.Max(mdPos.z, curPos.z); z++) {
                    GameUtils.createAndPlaceShape(shapePrefab, shapeType, ref gameEnv, new Vector3(x, curPos.y, z));
                }
            }
        } else if (mdPos == curPos) {
            GameUtils.createAndPlaceShape(shapePrefab, shapeType, ref gameEnv, curPos);
        }
    }

    void ShapeDelete(Vector3 curPos) {
        if (activeDrag) {
            for (int x = (int)(Mathf.Min(mdPos.x, curPos.x) / size); x <= (int)(Mathf.Max(mdPos.x, curPos.x) / size); x++) {
                for (int z = (int)Mathf.Min(mdPos.z, curPos.z); z <= (int)Mathf.Max(mdPos.z, curPos.z); z++) {
                    GameUtils.deleteShapeAtPos(new Vector3(x, curPos.y, z), ref mapScript);
                }
            }
        } else if (mdPos == curPos) {
            GameUtils.deleteShapeAtPos(curPos, ref mapScript);
        }
    }

    void shapeMode(Vector3 mPos) {
        // Calculate the 3d position of the index
        Vector3 curPos;
        curPos.x = Mathf.Floor(mPos.x);
        curPos.y = Mathf.Floor(mPos.y);
        curPos.z = Mathf.Floor(mPos.z);

        // Move the cursor to the current position
        placeCursor(curPos, 0);

        // Check mouse state and take actions
        if (Input.GetMouseButtonDown(0)) {
            startMouseDown(curPos);
        } else if (md && !activeDrag && curPos != mdPos) {
            shapeStartDrag(curPos);
        } else if (activeDrag) {
            shapeActiveDrag(curPos);
        } else if (Input.GetMouseButtonUp(0) && md) {
            if (deleteMode) {
	            ShapeDelete(curPos);
            } else {
                ShapePlace(curPos);
            }
            clearInput();
        } else if (!Input.GetMouseButton(0)) {
            clearInput();
        }
    }

    // ==================================
    // ================================== Wall Mode
    // ==================================
    void WallPlace(Vector3 wallPos, float wallRotation) {
        if (activeDrag) {
            if (wallPos.x - mdPos.x > wallPos.z - mdPos.z) {
                for (int x = (int)(Mathf.Min(mdPos.x, wallPos.x) / size); x <=(int)(Mathf.Max(mdPos.x, wallPos.x) / size); x++) {
                    GameUtils.createAndPlaceWall(wallPrefab, wallType, ref gameEnv, new Vector3(x, mdPos.y, mdPos.x), 0);
                }
            } else {
                for (int z = (int)(Mathf.Min(mdPos.z, wallPos.z) / size); z <=(int)(Mathf.Max(mdPos.z, wallPos.z) / size); z++) {
                    GameUtils.createAndPlaceWall(wallPrefab, wallType, ref gameEnv, new Vector3(mdPos.x, mdPos.y, z), 90);
                }
            }
        } else if (mdPos == wallPos) {
            GameUtils.createAndPlaceWall(wallPrefab, wallType, ref gameEnv, wallPos, wallRotation);
        }
    }

    void WallDelete(Vector3 wallPos) {
        if (activeDrag) {
            if (wallPos.x - mdPos.x > wallPos.z - mdPos.z) {
                for (int x = (int)(Mathf.Min(mdPos.x, wallPos.x) / size); x <=(int)(Mathf.Max(mdPos.x, wallPos.x) / size); x++) {
                    GameUtils.deleteWallAtPos(new Vector3(x, mdPos.y, mdPos.x), ref mapScript);
                }
            } else {
                for (int z = (int)(Mathf.Min(mdPos.z, wallPos.z) / size); z <=(int)(Mathf.Max(mdPos.z, wallPos.z) / size); z++) {
                    GameUtils.deleteWallAtPos(new Vector3(mdPos.x, mdPos.y, z), ref mapScript);
                }
            }
        } else if (mdPos == wallPos) {
            GameUtils.deleteWallAtPos(wallPos, ref mapScript);
        }
    }

    void wallMode(Vector3 mPos) {
        // Calculate position the wall should be at
        Vector3 wallPos;
        wallPos.y = Mathf.Floor(mPos.y);
        float wallRotation = 0;
        if (Math.Abs(Math.Round(mPos.x) - mPos.x) < Math.Abs(Math.Round(mPos.z) - mPos.z)) {
            wallPos.x = Mathf.Round(mPos.x) - 0.5f;
            wallPos.z = Mathf.Floor(mPos.z);
            wallRotation = 90;
        } else {
            wallPos.x = Mathf.Floor(mPos.x);
            wallPos.z = Mathf.Round(mPos.z) - 0.5f;
        }

        // Move the cursor to the current position
        placeCursor(wallPos, wallRotation);

        // Check mouse state and take actions
        if (Input.GetMouseButtonDown(0)) {
            startMouseDown(wallPos);
        } else if (md && !activeDrag && wallPos != mdPos) {
            wallStartDrag(wallPos, wallRotation);
        } else if (activeDrag) {
            wallActiveDrag(wallPos);
        } else if (Input.GetMouseButtonUp(0) && md) {
            if (deleteMode) {
	            WallPlace(wallPos, wallRotation);
            } else {
                WallDelete(wallPos);
            }
            clearInput();
        } else if (!Input.GetMouseButton(0)) {
            clearInput();
        }
    }

    // ==================================
    // ================================== Tile Mode
    // ==================================
    void tileMode(Vector3 mPos) {
        // TODO
    }

    // ======================================================================= START/UPDATE
    void runMapEditor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (layerCollider.Raycast(ray, out hitInfo, Mathf.Infinity)) {
            switch (mode) {
                case 0:
                    shapeMode(hitInfo.point);
                    break;
                case 1:
                    wallMode(hitInfo.point);
                    break;
                case 2:
                    tileMode(hitInfo.point);
                    break;
            }
        }
    }

    void Start() {
        gameEnv = gameEnvScript.gameEnv;
        size = mapScript.cubeSize;
        wallWidth = size * wallWidthMultiple;
        changeActiveLayer();
    }

    void Update() {
        runMapEditor();
    }
}
