using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgListScript : MonoBehaviour
{
    GameObject cubesObject;
    GameObject mobsObject;

	TypeTreeScript typeTreeScript;

    public Dictionary<string, Mob> mobDict = new Dictionary<string, Mob>();
    public Dictionary<string, Cube> cubeDict = new Dictionary<string, Cube>();

	public Dictionary<string, object[]> cubeParameters = new Dictionary<string, object[]>();
	public Dictionary<string, object[]> mobParameters = new Dictionary<string, object[]>();
    public Dictionary<string, object[]> relationalActionParameters = new Dictionary<string, object[]>();

    // Start is called before the first frame update
    void Start()
    {
        cubesObject = GameObject.Find("Cubes");
        mobsObject = GameObject.Find("Mobs");

    	typeTreeScript = GameObject.Find("GameLogic").GetComponent<TypeTreeScript>();

        cubeParameters.Add("testCube", new object[] {"testCube", "testCube", "cube", new List<string>(), new List<float>(), 1, 100});
        mobParameters.Add("testMob", new object[] {"testMob", "testMob", "mob", new List<string>(), new List<float>(), "Tester Mob", 1, 1, 1, 10, 100, 10, 10, 10, 10, 10, 10});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Mob createMob(object[] mobParams) {
        Mob newMob = mobsObject.AddComponent<Mob>();

        newMob.identifier = (string)mobParams[0];
        newMob.uniqueIdentifier = newMob.identifier + "_" + System.Guid.NewGuid().ToString();

        newMob.graphicAsset = (string)mobParams[1];

        newMob.type = typeTreeScript.typeDict[(string)mobParams[2]];
        List<string> mt = (List<string>)mobParams[3];
        for  (int i = 0; i < mt.Count; i++) {
                newMob.materialTypes.Add(typeTreeScript.typeDict[mt[i]]);
        }
        newMob.materialTypesDistribution = (List<float>)mobParams[4];

        newMob.width = (int)mobParams[6];
        newMob.length = (int)mobParams[7];
        newMob.height = (int)mobParams[8];

        newMob.boolVars.Add("cheapDiagMove", false);

        newMob.intVars.Add("hitScore", (int)mobParams[9]);
        newMob.intVars.Add("hitPoints", (int)mobParams[9]);
        newMob.intVars.Add("moveScore", (int)mobParams[10]);
        newMob.intVars.Add("movePoints", (int)mobParams[10]);
        newMob.intVars.Add("strength", (int)mobParams[11]);
        newMob.intVars.Add("dexterity", (int)mobParams[12]);
        newMob.intVars.Add("endurance", (int)mobParams[13]);
        newMob.intVars.Add("constitution", (int)mobParams[14]);
        newMob.intVars.Add("wisdom", (int)mobParams[15]);
        newMob.intVars.Add("intelligence", (int)mobParams[16]);

        return newMob;
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
