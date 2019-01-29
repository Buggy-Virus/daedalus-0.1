using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgListScript : MonoBehaviour
{
    GameObject cubesObject;
    GameObject mobsObject;

	TypeTreeScript typeTreeScript;

	public List<object[]> cubeParameterList = new List<object[]>();
	public List<object[]> mobParameterList = new List<object[]>();

    // Start is called before the first frame update
    void Start()
    {
        cubesObject = GameObject.Find("Cubes");
        mobsObject = GameObject.Find("Mobs");

    	typeTreeScript = GameObject.Find("GameLogic").GetComponent<TypeTreeScript>();

        cubeParameterList.Add(new object[] {"testCube", "testCubePrefab", "cube", new List<string>(), new List<float>(), 1, 0, 1, false, 100});
        mobParameterList.Add(new object[] {"testMob", "testMobPrefab", "mob", new List<string>(), new List<float>(), "Tester Mob", 1, 1, 1, 10, 100, 10, 10, 10, 10, 10, 10});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Cube createCube(object[] cubeParams) {  	
    	Cube newCube = cubesObject.AddComponent<Cube>();

        newCube.identifier = (string)cubeParams[0];
        newCube.uniqueIdentifier = newCube.identifier + "_" + System.Guid.NewGuid().ToString();

        newCube.graphicAsset = (string)cubeParams[1];

        newCube.mainType = typeTreeScript.typeDict[(string)cubeParams[2]];
        List<string> mt = (List<string>)cubeParams[3];
        for  (int i = 0; i < mt.Count; i++) {
                newCube.materialTypes.Add(typeTreeScript.typeDict[mt[i]]);
        }
        newCube.materialTypesDistribution = (List<float>)cubeParams[4];

        newCube.state = (int)cubeParams[5];
        newCube.transparency = (float)(int)cubeParams[6];
        newCube.viscosity = (int)cubeParams[7];
        newCube.flow = (bool)cubeParams[8];
        newCube.hitScore = (int)cubeParams[9];
        newCube.hitPoints = (int)cubeParams[9];

        return newCube;
    }

    public Mob createMob(object[] mobParams) {
        Mob newMob = mobsObject.AddComponent<Mob>();

        newMob.identifier = (string)mobParams[0];
        newMob.uniqueIdentifier = newMob.identifier + "_" + System.Guid.NewGuid().ToString();

        newMob.graphicAsset = (string)mobParams[1];

        newMob.mainType = typeTreeScript.typeDict[(string)mobParams[2]];
        List<string> mt = (List<string>)mobParams[3];
        for  (int i = 0; i < mt.Count; i++) {
                newMob.materialTypes.Add(typeTreeScript.typeDict[mt[i]]);
        }
        newMob.materialTypesDistribution = (List<float>)mobParams[4];

        newMob.width = (int)mobParams[6];
        newMob.length = (int)mobParams[7];
        newMob.height = (int)mobParams[8];
        newMob.hitScore = (int)mobParams[9];
        newMob.hitPoints = (int)mobParams[9];
        newMob.moveScore = (int)mobParams[10];
        newMob.movePoints = (int)mobParams[10];
        newMob.strength = (int)mobParams[11];
        newMob.dexterity = (int)mobParams[12];
        newMob.endurance = (int)mobParams[13];
        newMob.constitution = (int)mobParams[14];
        newMob.wisdom = (int)mobParams[15];
        newMob.intelligence = (int)mobParams[16];

        return newMob;
    }
}
