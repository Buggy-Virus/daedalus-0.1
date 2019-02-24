using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour {

    public int sizeX = 50;
    public int sizeZ = 50;
    public int sizeY = 4;

    public float cubeSize = 1;

    public GameCoord[,,] gameBoard;

    public GameObject layerPrefab;

    // Use this for initialization
    void Start() {
        for (int i = 0; i < sizeY; i++) {
            var layer = Instantiate(layerPrefab, new Vector3(transform.position.x, transform.position.y + i * cubeSize, transform.position.z), Quaternion.identity, transform);
            layer.name = "layer" + i.ToString();
        }

        gameBoard = new GameCoord[sizeX,sizeY,sizeZ];
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                for (int z = 0; z < sizeZ; z++) {
                    gameBoard[x, y, z] = gameObject.AddComponent<GameCoord>();
                    gameBoard[x, y, z].x = x;
                    gameBoard[x, y, z].y = y;
                    gameBoard[x, y, z].z = z;
                    gameBoard[x, y, z].tokens = new List<Token>();
                    gameBoard[x, y, z].characters = new List<Character>();
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        
	}
}
