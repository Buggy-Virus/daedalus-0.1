using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour {

    AssetScript assetScript;

    public int sizeX = 50;
    public int sizeZ = 50;
    public int sizeY = 4;

    public float cubeSize = 1;

    public GameTile[,,] gameMap;

    public GameObject targetPrefab;

    // Use this for initialization
    void Start() {
        assetScript = GameObject.Find("Assets").GetComponent<AssetScript>();

        for (int i = 0; i < sizeY; i++) {
            var layer = Instantiate(targetPrefab, new Vector3(transform.position.x, transform.position.y + i * cubeSize, transform.position.z), Quaternion.identity, transform);
            layer.name = "layer" + i.ToString();
        }

        gameMap = new GameTile[sizeX,sizeY,sizeZ];
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                for (int z = 0; z < sizeZ; z++) {
                    gameMap[x, y, z] = new GameTile(assetScript.tiles[0], x, y, z);
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        
	}
}
