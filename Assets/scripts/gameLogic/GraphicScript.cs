using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicScript : MonoBehaviour
{
	public List<GameObject> cubePrefabs;
	public List<GameObject> mobPrefabs;

	public GameObject testCubePrefab;
	public GameObject testMobPrefab;
    // Start is called before the first frame update
    void Start()
    {
    	cubePrefabs.Add(testCubePrefab);
    	mobPrefabs.Add(testMobPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
