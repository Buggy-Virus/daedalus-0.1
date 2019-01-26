using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class MouseControls : MonoBehaviour {

    MapScript mapScript;
    AssetScript assetScript;

    string elapsedTicks;

    GameObject activeLayerObject;
    LayerScript activelayerScript;
    Collider activeLayerCollider;

    public Dictionary<string, int> mouseDownIndex;
    public Dictionary<string, int> mouseUpLocation;
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

    moveTile[,,] moveMap;

    public Dictionary<Token, GameObject> tokenToObject;
    public Dictionary<GameObject, Token> objectToToken;

    public int activeLayer = 0;
    int lastActiveLayer = -1;

    public int controlMode = 0;
    int lastControlMode = -1;

    public int tileType = 0;
    int lastTileType = -1;

    public int thingType = 0;
    int lastThingType = -1;

    public int tokenType = 0;
    int lastTokenType = -1;

    void Start() {
        tokenToObject = new Dictionary<Token, GameObject>();
        objectToToken = new Dictionary<GameObject, Token>();

        mapScript = GameObject.Find("Map").GetComponent<MapScript>();
        assetScript = GameObject.Find("Assets").GetComponent<AssetScript>();
    }

    // Update is called once per frame
    void Update() {
        updateActiveLayer();

        trackTime();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;     
        
        if (activeLayerCollider.Raycast(ray, out hitInfo, Mathf.Infinity)) {

            int x = Mathf.FloorToInt(hitInfo.point.x / mapScript.cubeSize);
            int z = Mathf.FloorToInt(hitInfo.point.z / mapScript.cubeSize);
            int y = Mathf.FloorToInt(hitInfo.point.y / mapScript.cubeSize);

            Dictionary<string, int> currentIndex = new Dictionary<string, int>();
            currentIndex.Add("x", (int) x);
            currentIndex.Add("y", (int) y);
            currentIndex.Add("z", (int) z);

            Vector3 currentCoord;
            currentCoord.x = x * mapScript.cubeSize;
            currentCoord.y = y * mapScript.cubeSize;
            currentCoord.z = z * mapScript.cubeSize;

            switch (controlMode) {
                case 0:
                    tileMode(currentCoord, currentIndex);
                    break;
                case 1:
                    thingMode(currentCoord, currentIndex);
                    break;
                case 2:
                    tokenMode(currentCoord, currentIndex);
                    break;
                case 3:
                    selectMode(currentCoord, currentIndex);
                    break;
                case 4:
                    moveMode(currentCoord, currentIndex);
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
            activelayerScript = activeLayerObject.GetComponent<LayerScript>();
            activeLayerCollider = activeLayerObject.GetComponent<MeshCollider>();

            lastActiveLayer = activeLayer;
        }
    }

    void trackTime() {
        System.DateTime timeBegin = new System.DateTime(2018, 6, 8);
        System.DateTime now = System.DateTime.Now;
        elapsedTicks = (timeBegin.Ticks - now.Ticks).ToString();
    }

    void tileMode(Vector3 position, Dictionary<string, int> index) {
        if (lastControlMode != 0 || lastTileType != tileType) {
            Debug.Log("tile mode");
            try {
                Destroy(cursorObject);
            } catch { }
            cursorObject = Instantiate(assetScript.tileObjects[tileType], position, Quaternion.identity, gameObject.transform);
            cursorObjectRenderer = cursorObject.transform.GetChild(0).GetComponent<MeshRenderer>();
            cursorObjectRenderer.material.color = cursorTint;
            cursorObject.name = "cursorTile";

            activeCursor = true;
            lastControlMode = controlMode;
            lastTileType = tileType;
        }

        if (mouseDown && position != mouseDownPosition && !activeDrag) {
            activeDrag = true;
            Vector3 dragObjectOrigin = new Vector3(mouseDownPosition.x + mapScript.cubeSize / 2, mouseDownPosition.y + mapScript.cubeSize / 2, mouseDownPosition.z + mapScript.cubeSize / 2);
            dragObject = Instantiate(dragPrefab, dragObjectOrigin, Quaternion.identity, gameObject.transform);
            dragObject.name = "DragObject";
        }

        if (activeDrag) {
            dragObject.transform.localScale = new Vector3(Mathf.Abs(mouseDownPosition.x - position.x) + mapScript.cubeSize, dragObject.transform.localScale.y, Mathf.Abs(mouseDownPosition.z - position.z) + mapScript.cubeSize);
            dragObject.transform.position = new Vector3((mouseDownPosition.x + position.x + mapScript.cubeSize) / 2, dragObject.transform.position.y, (mouseDownPosition.z + position.z + mapScript.cubeSize) / 2);
        }

        if (Input.GetMouseButtonDown(0)) {
            cursorObjectRenderer.material.color = cursorClickHighlight;
            mouseDownPosition = position;
            mouseDownIndex = index;
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            Dictionary<string, int> curIndex;
            if (activeDrag) {
                for (int x = (int) Mathf.Min(mouseDownPosition.x, position.x); x <= (int) Mathf.Max(mouseDownPosition.x, position.x); x++) {
                    for (int z = (int)Mathf.Min(mouseDownPosition.z, position.z); z <= (int)Mathf.Max(mouseDownPosition.z, position.z); z++) {
                        curIndex = new Dictionary<string, int>();
                        curIndex.Add("x", x);
                        curIndex.Add("y", index["y"]);
                        curIndex.Add("z", z);
                        AlterTile(new Vector3(x, position.y, z), curIndex, tileType);
                    }
                }
                Destroy(dragObject);
            } else if (mouseDown && mouseDownIndex == index) {
                AlterTile(position, index, tileType);
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
    }

    void AlterTile(Vector3 position, Dictionary<string, int> index, int tileType) {
        TileDT curTileDT = assetScript.tiles[tileType];
        GameTile curGameTile = mapScript.gameMap[index["x"], index["y"], index["z"]];
        if (curGameTile.tileType != 0) {
            Destroy(GameObject.Find(assetScript.tiles[curGameTile.tileType].tileString + " " + curGameTile.coord));
        }
        curGameTile.tileType = tileType;
        curGameTile.tileDT = curTileDT;
        string curTileName = curTileDT.tileString + " " + curGameTile.coord;
        curGameTile.tileName = curTileName;
        if (tileType != 0) {
            curGameTile.rendered = true;
            GameObject graphicTile = Instantiate(assetScript.tileObjects[tileType], position, Quaternion.identity, activeLayerObject.transform);
            graphicTile.name = curTileName;
        } else if (tileType == 0) {
            curGameTile.rendered = false;
        }
    }

    void thingMode(Vector3 position, Dictionary<string, int> index) {
        if (lastControlMode != 1 || lastThingType != thingType) {
            Debug.Log("thing mode");
            try {
                Destroy(gameObject.transform.GetChild(0).gameObject);
            } catch { }
            cursorObject = Instantiate(assetScript.thingObjects[thingType], position, Quaternion.identity, gameObject.transform);
            cursorObjectRenderer = cursorObject.transform.GetChild(0).GetComponent<MeshRenderer>();
            cursorObjectRenderer.material.color = cursorTint;
            cursorObject.name = "cursorThing";

            activeCursor = true;
            lastControlMode = controlMode;
            lastThingType = thingType;
        }

        if (Input.GetMouseButtonDown(0)) {
            cursorObjectRenderer.material.color = cursorClickHighlight;
        }

        if (Input.GetMouseButtonUp(0) && thingType != 0) {
            ThingDT curThingDT = assetScript.things[thingType];
            GameTile curGameTile = mapScript.gameMap[index["x"], index["y"], index["z"]];
            Thing newThing = new Thing(curThingDT);
            curGameTile.things.Add(newThing);
            GameObject graphicThing = Instantiate(assetScript.thingObjects[thingType], position, Quaternion.identity, activeLayerObject.transform);
            graphicThing.name = newThing.thingName;
        }

        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
            cursorObjectRenderer.material.color = normalColor;
        }
    }

    void tokenMode(Vector3 position, Dictionary<string, int> index) {
        if (lastControlMode != 2 || lastTokenType != tokenType) {
            Debug.Log("token mode");
            try {
                Destroy(cursorObject);
            } catch { }
            cursorObject = Instantiate(assetScript.tokenObjects[tokenType], position, Quaternion.identity, gameObject.transform);
            cursorObjectRenderer = cursorObject.transform.GetChild(0).GetComponent<MeshRenderer>();
            cursorObjectRenderer.material.color = cursorTint;
            cursorObject.name = "cursorToken";

            activeCursor = true;
            lastControlMode = controlMode;
            lastTokenType = tokenType;
        }

        if (Input.GetMouseButtonDown(0)) {
            cursorObjectRenderer.material.color = cursorClickHighlight;
        }

        if (Input.GetMouseButtonUp(0) && tokenType != 0) {
            TokenDT curTokenDT = assetScript.tokens[tokenType];
            GameTile curGameTile = mapScript.gameMap[index["x"], index["y"], index["z"]];
            Token newToken = new Token(curTokenDT, "Thom" + elapsedTicks, 5, curGameTile);
            curGameTile.tokens.Add(newToken);
            GameObject graphicObject = Instantiate(assetScript.tokenObjects[tokenType], position, Quaternion.identity, activeLayerObject.transform);
            graphicObject.name = newToken.name;
            GraphicTokenScript graphicTokenScript = graphicObject.GetComponent<GraphicTokenScript>();
            graphicTokenScript.gameToken = newToken;
            tokenToObject.Add(newToken, graphicObject);
            objectToToken.Add(graphicObject, newToken);
        }

        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
            cursorObjectRenderer.material.color = normalColor;
        }
    }

    void cursorlessControlMode () {
        try {
            Destroy(cursorObject);
        } catch { }
        activeCursor = false;
        lastControlMode = controlMode;
    }

    void selectMode(Vector3 position, Dictionary<string, int> index) {
        if (lastControlMode != 3) {
            Debug.Log("select mode");
            cursorlessControlMode();
        }
    }

    void moveMode(Vector3 position, Dictionary<string, int> index) {
        // Add movement on diagnols
        // Switch to using djikstra's
        if (lastControlMode != 4) {
            Debug.Log("select mode");
            cursorlessControlMode();

            GameTile curTile = selectedToken.curTile;
            int curX = curTile.x;
            int curY = curTile.y;
            int curZ = curTile.z;
            moveMap = new moveTile[mapScript.sizeX,mapScript.sizeY,mapScript.sizeZ];
            populateMoveMap(curX, curY, curZ);
        }

        if (Input.GetMouseButtonDown(0)) {
            mouseDownIndex = index;
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            if (index == mouseDownIndex && mouseDown) {
                moveTile potentialTile = moveMap[index["x"], index["y"], index["z"]];
                if (potentialTile.accessible && potentialTile.visited) {
                    GameTile oldGameTile = selectedToken.curTile;
                    GameTile newGameTile = mapScript.gameMap[index["x"], index["y"], index["z"]];
                    oldGameTile.tokens.Remove(selectedToken);
                    newGameTile.tokens.Add(selectedToken);
                    selectedToken.curTile = newGameTile;
                    selectedToken.movePoints -= potentialTile.moveCost;

                    selectedObject.transform.position = position;
                } else {
                    Debug.Log("Tile Not Accessible.");
                }
            }
            mouseDown = false;
        }

        if (!Input.GetMouseButton(0)) {
            mouseDown = false;
        }
    }

    void populateMoveMap(int x, int y, int z) {
        moveTile curTile = moveMap[x, y, z];
        curTile.visited = true;
        curTile.moveCost = 0;
        curTile.accessible = false;

        checkAdjacent(curTile, x + 1, y, z);
        checkAdjacent(curTile, x - 1, y, z);
        checkAdjacent(curTile, x, y + 1, z);
        checkAdjacent(curTile, x, y - 1, z);

        traverseAdjacent(x, y, z);
        traverseAdjacent(x, y, z);
        traverseAdjacent(x, y, z);
        traverseAdjacent(x, y, z);
    }

    void traverseAdjacent(int x, int y, int z) {
        moveTile curTile = moveMap[x,y,z];
        if (curTile.moveCost <= selectedToken.movePoints && curTile.accessible && !curTile.visited) {
            checkAdjacent(curTile, x + 1, y, z);
            checkAdjacent(curTile, x - 1, y, z);
            checkAdjacent(curTile, x, y + 1, z);
            checkAdjacent(curTile, x, y - 1, z);

            traverseAdjacent(x, y, z);
            traverseAdjacent(x, y, z);
            traverseAdjacent(x, y, z);
            traverseAdjacent(x, y, z);
        }
    }

    void checkAdjacent (moveTile prevTile, int x, int y, int z) {
        moveTile curTile = moveMap[x, y, z];
        int checkCost = prevTile.moveCost + 1;
        if (mapScript.gameMap[x, y, z].tileDT.passable) {
            if (checkCost < curTile.moveCost) {
                curTile.moveCost = checkCost;
                curTile.prevTile = prevTile;
            }
        } else {
            curTile.accessible = false;
            curTile.visited = true;
        }
    }
}

