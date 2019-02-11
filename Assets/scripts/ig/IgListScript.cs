using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgListScript : MonoBehaviour
{
    GameObject cubesObject;
    GameObject tokensObject;

	TypeTreeScript typeTreeScript;

    public Dictionary<string, Token> tokenDict = new Dictionary<string, Token>();
    public Dictionary<string, Cube> cubeDict = new Dictionary<string, Cube>();

	public Dictionary<string, object[]> cubeParameters = new Dictionary<string, object[]>();
	public Dictionary<string, object[]> tokenParameters = new Dictionary<string, object[]>();
    public Dictionary<string, object[]> relationalActionParameters = new Dictionary<string, object[]>();

    // Start is called before the first frame update
    void Start()
    {
        cubesObject = GameObject.Find("Cubes");
        tokensObject = GameObject.Find("Tokens");

    	typeTreeScript = GameObject.Find("GameLogic").GetComponent<TypeTreeScript>();

        cubeParameters.Add("testCube", new object[] {"testCube", "testCube", "cube", new List<string>(), new List<float>(), 1, 100});
        tokenParameters.Add("testToken", new object[] {"testToken", "testToken", "token", new List<string>(), new List<float>(), "Tester Token", 1, 1, 1, 10, 100, 10, 10, 10, 10, 10, 10});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Token createToken(object[] tokenParams) {
        Token newToken = tokensObject.AddComponent<Token>();

        newToken.identifier = (string)tokenParams[0];
        newToken.uniqueIdentifier = newToken.identifier + "_" + System.Guid.NewGuid().ToString();

        newToken.graphicAsset = (string)tokenParams[1];

        newToken.type = typeTreeScript.typeDict[(string)tokenParams[2]];
        List<string> mt = (List<string>)tokenParams[3];
        for  (int i = 0; i < mt.Count; i++) {
                newToken.materialTypes.Add(typeTreeScript.typeDict[mt[i]]);
        }
        newToken.materialTypesDistribution = (List<float>)tokenParams[4];

        newToken.width = (int)tokenParams[6];
        newToken.length = (int)tokenParams[7];
        newToken.height = (int)tokenParams[8];

        newToken.boolVars.Add("cheapDiagMove", false);

        newToken.intVars.Add("hitScore", (int)tokenParams[9]);
        newToken.intVars.Add("hitPoints", (int)tokenParams[9]);
        newToken.intVars.Add("moveScore", (int)tokenParams[10]);
        newToken.intVars.Add("movePoints", (int)tokenParams[10]);
        newToken.intVars.Add("strength", (int)tokenParams[11]);
        newToken.intVars.Add("dexterity", (int)tokenParams[12]);
        newToken.intVars.Add("endurance", (int)tokenParams[13]);
        newToken.intVars.Add("constitution", (int)tokenParams[14]);
        newToken.intVars.Add("wisdom", (int)tokenParams[15]);
        newToken.intVars.Add("intelligence", (int)tokenParams[16]);

        return newToken;
    }

    public Cube createCube(object[] cubeParams) {   
        Cube newCube = cubesObject.AddComponent<Cube>();

        newCube.identifier = (string)cubeParams[0];
        newCube.uniqueIdentifier = newCube.identifier + "_" + System.Guid.NewGuid().ToString();

        newCube.graphicAsset = (string)cubeParams[1];

        newCube.type = typeTreeScript.typeDict[(string)cubeParams[2]];
        List<string> mt = (List<string>)cubeParams[3];
        for  (int i = 0; i < mt.Count; i++) {
                newCube.materialTypes.Add(typeTreeScript.typeDict[mt[i]]);
        }
        newCube.materialTypesDistribution = (List<float>)cubeParams[4];

        newCube.intVars.Add("state", (int)cubeParams[5]);
        newCube.intVars.Add("hitScore", (int)cubeParams[6]);
        newCube.intVars.Add("hitPoints", (int)cubeParams[6]);

        return newCube;
    }
    
}
