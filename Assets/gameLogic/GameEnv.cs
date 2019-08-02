using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnv
{
    public int inGameTime;

    public float cubeSize;

    public GameObject tokensObject;
    public GameObject cubesObject;

    public Dictionary<string, GameObject> tokenDict;
    public Dictionary<string, GameObject> cubeDict;

    public GameCoord[,,] gameBoard;

    public GameEnv(){
        tokenDict = new Dictionary<string, GameObject>();
        cubeDict = new Dictionary<string, GameObject>();
    }
}