using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicScript : MonoBehaviour
{
	public Dictionary<string, GameObject> cubePrefabs = new Dictionary<string, GameObject>();
	public Dictionary<string, GameObject> tokenPrefabs = new Dictionary<string, GameObject>();

	public GameObject testCubePrefab;
	public GameObject testTokenPrefab;
    // Start is called before the first frame update
    void Start()
    {
    	cubePrefabs.Add("testCube", testCubePrefab);
    	tokenPrefabs.Add("testToken", testTokenPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
