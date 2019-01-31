using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using UnityEngine;

public class MouseControls : MonoBehaviour {

	GameObject cubesObject;
    GameObject mobsObject;

    MapScript mapScript;
    GraphicScript graphicScript;
    IgListScript igListScript;

    public GameObject activeLayerObject;
    LayerScript activelayerScript;
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

    public Mob selectedMob;
    public GameObject selectedObject;

    //moveTile[,,] moveMap;

    public int activeLayer = 0;
    int lastActiveLayer = -1;

    public int controlMode = 0;
    int lastControlMode = -1;

    public bool deleteMode = false;

    public string cubeType = "testCube";
    string lastCubeType = "testCube";

    public string mobType = "testMob";
    string lastMobType = "testMob";

    bool usedDiagnolMove;

    void Start() {
    	cubeType = "testCube";
    	mobType = "testMob";
    	Debug.Log(cubeType);
    	cubesObject = GameObject.Find("Cubes");
        mobsObject = GameObject.Find("Mobs");

        mapScript = GameObject.Find("Map").GetComponent<MapScript>();
        graphicScript = GameObject.Find("GameLogic").GetComponent<GraphicScript>();
        igListScript = GameObject.Find("GameLogic").GetComponent<IgListScript>();

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
                case 2:
                    mobMode(currentCoord, currentIndex);
                    break;
                case 3:
                    selectMode();
                    break;
                case 4:
                	moveMode(currentCoord, currentIndex);
                	break;
                // case 4:
                //     moveMode(currentCoord, currentIndex);
                //     break;
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

    void cubeMode(Vector3 curPosition, Index curIndex) {
        if (lastControlMode != 0 || lastCubeType != cubeType) {
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
	            mouseDownIndex = curIndex;
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
	            } else if (indexEqual(mouseDownIndex, curIndex)) {
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
	            mouseDownIndex = curIndex;
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
	            } else if (indexEqual(mouseDownIndex, curIndex)) {
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
            Destroy(gameCoord.cube.robject);
            Destroy(gameCoord.cube);
        }
        Cube newCube = igListScript.createCube(igListScript.cubeParameters[cubeType]);
        newCube.index = curIndex;

        gameCoord.cube = newCube;

        Debug.Log(graphicScript.cubePrefabs[cubeType]);
        GameObject graphicCube = Instantiate(graphicScript.cubePrefabs[cubeType], curPosition, Quaternion.identity, cubesObject.transform);

        newCube.robject = graphicCube;
        graphicCube.GetComponent<CubePrefabScript>().cube = newCube;
        graphicCube.name = newCube.uniqueIdentifier;
    }

    void deleteCube(Index curIndex) {
    	GameCoord gameCoord = mapScript.gameBoard[curIndex.x, curIndex.y, curIndex.z];
    	if (gameCoord.cube != null) {
            Destroy(gameCoord.cube.robject);
        }
    	gameCoord.cube = null;
    }

    void mobMode(Vector3 curPosition, Index curIndex) {
        if (lastControlMode != 2 || lastMobType != mobType) {
            Debug.Log("mob mode");
            destroyCursor();
            addCursor(curPosition, graphicScript.mobPrefabs[mobType], "cursorMob");
            lastControlMode = controlMode;
            lastMobType = mobType;
        }

        if (!deleteMode) {
	        if (Input.GetMouseButtonDown(0)) {
	            cursorObjectRenderer.material.color = cursorClickHighlight;
	        }

	        if (Input.GetMouseButtonUp(0)) {
	            placeMob(curPosition, curIndex, mobType);
	        }

	        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
	            cursorObjectRenderer.material.color = normalColor;
	        }
        }
    }

    void placeMob(Vector3 curPosition, Index curIndex, string mobType) {
    	Debug.Log("Placed Mob");
    	GameCoord gameCoord = mapScript.gameBoard[curIndex.x, curIndex.y, curIndex.z];

    	Mob newMob = igListScript.createMob(igListScript.mobParameters[mobType]);
    	newMob.index = curIndex;

    	gameCoord.mobs.Add(newMob);

    	GameObject graphicMob = Instantiate(graphicScript.mobPrefabs[mobType], curPosition, Quaternion.identity, mobsObject.transform);
    	
    	newMob.robject = graphicMob;
    	graphicMob.GetComponent<MobPrefabScript>().mob = newMob;
    	graphicMob.name = newMob.uniqueIdentifier;
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
        if (lastControlMode != 3) {
            Debug.Log("select mode");
            cursorlessControlMode();
        }
    }

    bool indexEqual(Index ind1, Index ind2) {
		return (ind1.x == ind2.x && ind1.y == ind2.y && ind1.z == ind2.z);
	}

	void moveMode(Vector3 curPosition, Index curIndex) {
		if (lastControlMode != 4) {
            Debug.Log("move mode");
            destroyCursor();
            addCursor(curPosition, graphicScript.mobPrefabs[selectedMob.graphicAsset], "cursorMob");
            lastControlMode = controlMode;
        }

        if (Input.GetMouseButtonDown(0)) {
	            cursorObjectRenderer.material.color = cursorClickHighlight;
	    }

	    if (Input.GetMouseButtonUp(0)) {
           	Index startIndex = selectedMob.index;
           	if (startIndex.z == curIndex.z && Math.Abs(startIndex.x - curIndex.x) <= selectedMob.movePoints) {
           		displaceMob(selectedMob, curPosition, curIndex);
           		selectedMob.movePoints -= Math.Abs(startIndex.x - curIndex.x);
           	} else if (startIndex.x == curIndex.x && Math.Abs(startIndex.z - curIndex.z) <= selectedMob.movePoints) {
           		displaceMob(selectedMob, curPosition, curIndex);
           		selectedMob.movePoints -= Math.Abs(startIndex.z - curIndex.z);
           	} else if (Math.Abs(startIndex.z - curIndex.z) == Math.Abs(startIndex.x - curIndex.x)) {
           		int diagnolDisplacement = Math.Abs(startIndex.z - curIndex.z);
           		int moveCost;
           		if (diagnolDisplacement % 2 == 0) {
           			moveCost = (int)((float)diagnolDisplacement * 1.5);
           		} else if (usedDiagnolMove) { 
           			moveCost = (int)(((float)diagnolDisplacement - 1) * 1.5 + 2);
           			usedDiagnolMove = false;
           		} else {
           			moveCost = (int)(((float)diagnolDisplacement - 1) * 1.5 + 1);
           			usedDiagnolMove = true;
           		}
           		if (moveCost <= selectedMob.movePoints) {
           			displaceMob(selectedMob, curPosition, curIndex);
           			selectedMob.movePoints -= moveCost;
           		} else {
           			Debug.Log("Not enough movepoints");
           		}
           	} else if (
           			(startIndex.z == curIndex.z && Math.Abs(startIndex.x - curIndex.x) > selectedMob.movePoints)
           			|| (startIndex.x == curIndex.x && Math.Abs(startIndex.z - curIndex.z) > selectedMob.movePoints)
           		) {
           		Debug.Log("Not enough movepoints");
           	} else {
           		Debug.Log("Not a valid move square");
           	}
	    }

	    if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
	        cursorObjectRenderer.material.color = normalColor;
	    }
	}

	void displaceMob(Mob curMob, Vector3 destPosition, Index destIndex) {
		Index startIndex = curMob.index;
		GameCoord startGameCoord = mapScript.gameBoard[startIndex.x, startIndex.y, startIndex.z];
		GameCoord destGameCoord = mapScript.gameBoard[destIndex.x, destIndex.y, destIndex.z];
		GameObject mobObject = curMob.robject;

		startGameCoord.mobs.Remove(curMob);
		destGameCoord.mobs.Add(curMob);

		mobObject.transform.position = destPosition;
		curMob.index = destIndex;

	}

    // void moveMode(Vector3 position, Dictionary<string, int> index) {
    //     // Add movement on diagnols
    //     // Switch to using djikstra's
    //     if (lastControlMode != 4) {
    //         Debug.Log("select mode");
    //         cursorlessControlMode();

    //         GameTile curTile = selectedToken.curTile;
    //         int curX = curTile.x;
    //         int curY = curTile.y;
    //         int curZ = curTile.z;
    //         moveMap = new moveTile[mapScript.sizeX,mapScript.sizeY,mapScript.sizeZ];
    //         populateMoveMap(curX, curY, curZ);
    //     }

    //     if (Input.GetMouseButtonDown(0)) {
    //         mouseDownIndex = index;
    //         mouseDown = true;
    //     }

    //     if (Input.GetMouseButtonUp(0)) {
    //         if (index == mouseDownIndex && mouseDown) {
    //             moveTile potentialTile = moveMap[index["x"], index["y"], index["z"]];
    //             if (potentialTile.accessible && potentialTile.visited) {
    //                 GameTile oldGameTile = selectedToken.curTile;
    //                 GameTile newGameTile = mapScript.gameMap[index["x"], index["y"], index["z"]];
    //                 oldGameTile.tokens.Remove(selectedToken);
    //                 newGameTile.tokens.Add(selectedToken);
    //                 selectedToken.curTile = newGameTile;
    //                 selectedToken.movePoints -= potentialTile.moveCost;

    //                 selectedObject.transform.position = position;
    //             } else {
    //                 Debug.Log("Tile Not Accessible.");
    //             }
    //         }
    //         mouseDown = false;
    //     }

    //     if (!Input.GetMouseButton(0)) {
    //         mouseDown = false;
    //     }
    // }

    // void populateMoveMap(int x, int y, int z) {
    //     moveTile curTile = moveMap[x, y, z];
    //     curTile.visited = true;
    //     curTile.moveCost = 0;
    //     curTile.accessible = false;

    //     checkAdjacent(curTile, x + 1, y, z);
    //     checkAdjacent(curTile, x - 1, y, z);
    //     checkAdjacent(curTile, x, y + 1, z);
    //     checkAdjacent(curTile, x, y - 1, z);

    //     traverseAdjacent(x, y, z);
    //     traverseAdjacent(x, y, z);
    //     traverseAdjacent(x, y, z);
    //     traverseAdjacent(x, y, z);
    // }

    // void traverseAdjacent(int x, int y, int z) {
    //     moveTile curTile = moveMap[x,y,z];
    //     if (curTile.moveCost <= selectedToken.movePoints && curTile.accessible && !curTile.visited) {
    //         checkAdjacent(curTile, x + 1, y, z);
    //         checkAdjacent(curTile, x - 1, y, z);
    //         checkAdjacent(curTile, x, y + 1, z);
    //         checkAdjacent(curTile, x, y - 1, z);

    //         traverseAdjacent(x, y, z);
    //         traverseAdjacent(x, y, z);
    //         traverseAdjacent(x, y, z);
    //         traverseAdjacent(x, y, z);
    //     }
    // }

    // void checkAdjacent (moveTile prevTile, int x, int y, int z) {
    //     moveTile curTile = moveMap[x, y, z];
    //     int checkCost = prevTile.moveCost + 1;
    //     if (mapScript.gameMap[x, y, z].tileDT.passable) {
    //         if (checkCost < curTile.moveCost) {
    //             curTile.moveCost = checkCost;
    //             curTile.prevTile = prevTile;
    //         }
    //     } else {
    //         curTile.accessible = false;
    //         curTile.visited = true;
    //     }
    // }
}

