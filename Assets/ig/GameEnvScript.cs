using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnvScript : MonoBehaviour
{
    public GameObject testCubePrefab;
	public GameObject testTokenPrefab;

    GameObject cubesObject;
    GameObject tokensObject;

    public GameEnv gameEnv;
    int lastGameTime;

    Dictionary<string, Type> typeDict;

    public List<Event> gameTimeEvents;
    public List<Event> realTimeEvents;

	public Dictionary<string, object[]> cubeParameters = new Dictionary<string, object[]>();
	public Dictionary<string, object[]> tokenParameters = new Dictionary<string, object[]>();
    public Dictionary<string, object[]> relationalActionParameters = new Dictionary<string, object[]>();

    public Dictionary<string, GameObject> cubePrefabs = new Dictionary<string, GameObject>();
	public Dictionary<string, GameObject> tokenPrefabs = new Dictionary<string, GameObject>();

    void Start()
    {
        typeDict = new Dictionary<string, Type>();
        typeDict.Add("testCube", new Type("cube"));
        typeDict.Add("testToken", new Type("token"));

        cubePrefabs.Add("testCube", testCubePrefab);
    	tokenPrefabs.Add("testToken", testTokenPrefab);

        gameEnv = new GameEnv();

        gameEnv.cubesObject = GameObject.Find("Cubes");
        gameEnv.tokensObject = GameObject.Find("Tokens");

        cubeParameters.Add("testCube", new object[] {"testCube", "testCube", "cube", new List<string>(), new List<float>(), 1, 100});
        tokenParameters.Add("testToken", new object[] {"testToken", "testToken", "token", new List<string>(), new List<float>(), "Tester Token", 1, 1, 1, 10, 100, 10, 10, 10, 10, 10, 10});
    }

    void Update()
    {
        // foreach (Event realTimeEvent in realTimeEvents) {
        //     // TODO
        //     resolveEvent(realTimeEvent, ref gameEnv);
        //     removeEndedEvent(realTimeEvent, ref realTimeEvents);
        // }

        // if (gameEnv.inGameTime != lastGameTime) {
        //     lastGameTime = gameEnv.inGameTime;
        //     foreach (Event gameTimeEvent in gameTimeEvents) {
        //         ResolveEventArgs(gameTimeEvent, ref gameEnv);
        //         removeEndedEvent(gameTimeEvent, ref gameTimeEvents);
        //     }
        // }
    }

    // public Token createToken(object[] tokenParams) {
    //     Token newToken = tokensObject.AddComponent<Token>();

    //     newToken.identifier = (string)tokenParams[0];
    //     newToken.uniqueIdentifier = newToken.identifier + "_" + System.Guid.NewGuid().ToString();

    //     newToken.graphicAsset = (string)tokenParams[1];

    //     newToken.type = typeDict[(string)tokenParams[2]];
    //     List<string> mt = (List<string>)tokenParams[3];
    //     for  (int i = 0; i < mt.Count; i++) {
    //             newToken.materialTypes.Add(typeDict[mt[i]]);
    //     }
    //     newToken.materialTypesDistribution = (List<float>)tokenParams[4];

    //     newToken.width = (int)tokenParams[6];
    //     newToken.length = (int)tokenParams[7];
    //     newToken.height = (int)tokenParams[8];

    //     newToken.boolVars.Add("cheapDiagMove", false);

    //     newToken.intVars.Add("hitScore", (int)tokenParams[9]);
    //     newToken.intVars.Add("hitPoints", (int)tokenParams[9]);
    //     newToken.intVars.Add("moveScore", (int)tokenParams[10]);
    //     newToken.intVars.Add("movePoints", (int)tokenParams[10]);
    //     newToken.intVars.Add("strength", (int)tokenParams[11]);
    //     newToken.intVars.Add("dexterity", (int)tokenParams[12]);
    //     newToken.intVars.Add("endurance", (int)tokenParams[13]);
    //     newToken.intVars.Add("constitution", (int)tokenParams[14]);
    //     newToken.intVars.Add("wisdom", (int)tokenParams[15]);
    //     newToken.intVars.Add("intelligence", (int)tokenParams[16]);

    //     return newToken;
    // }

    // public Cube createCube(object[] cubeParams) {   
    //     Cube newCube = cubesObject.AddComponent<Cube>();

    //     newCube.identifier = (string)cubeParams[0];
    //     newCube.uniqueIdentifier = newCube.identifier + "_" + System.Guid.NewGuid().ToString();

    //     newCube.graphicAsset = (string)cubeParams[1];

    //     newCube.type = typeDict[(string)cubeParams[2]];
    //     List<string> mt = (List<string>)cubeParams[3];
    //     for  (int i = 0; i < mt.Count; i++) {
    //             newCube.materialTypes.Add(typeDict[mt[i]]);
    //     }
    //     newCube.materialTypesDistribution = (List<float>)cubeParams[4];

    //     newCube.intVars.Add("state", (int)cubeParams[5]);
    //     newCube.intVars.Add("hitScore", (int)cubeParams[6]);
    //     newCube.intVars.Add("hitPoints", (int)cubeParams[6]);

    //     return newCube;
    // }
    
}
