using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetScript : MonoBehaviour {

    public List<TileDT> tiles = new List<TileDT>();
    public List<ThingDT> things = new List<ThingDT>();
    public List<TokenDT> tokens = new List<TokenDT>();

    public List<GameObject> tileObjects = new List<GameObject>();
    public List<GameObject> thingObjects = new List<GameObject>();
    public List<GameObject> tokenObjects = new List<GameObject>();

    // Hardcoded in Tiles and Things

    public GameObject tilePrefab;
    public GameObject waterPrefab;

    public GameObject boxPrefab;
    public GameObject columnPrefab;

    public GameObject capsulePrefab;

    // Use this for initialization
    void Start () {
        AddTileDT(new TileDT("Empty", false), tilePrefab);
        AddThingDT(new ThingDT("null"), boxPrefab);
        AddTokenDT(new TokenDT("null"), capsulePrefab);

        // Hardcoded in Tiles and Things being added
        AddTileDT(new TileDT("Rock", true), tilePrefab);
        AddTileDT(new TileDT("Water", false), waterPrefab);

        AddThingDT(new ThingDT("Box"), boxPrefab);
        AddThingDT(new ThingDT("Column"), columnPrefab);

        AddTokenDT(new TokenDT("Capsule Man"), capsulePrefab);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddTileDT (TileDT t, GameObject o) {
        tiles.Add(t);
        tileObjects.Add(o);
    }

    void AddThingDT (ThingDT t, GameObject o) {
        things.Add(t);
        thingObjects.Add(o);
    }

    void AddTokenDT (TokenDT t, GameObject o) {
        tokens.Add(t);
        tokenObjects.Add(o);
    }
}
