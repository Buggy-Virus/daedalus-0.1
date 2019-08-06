using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnv
{
    public int inGameTime;

    public float cubeSize;

    public MapScript mapScript;

    public GameObject tokensObject;
    public GameObject cubesObject;

    public Dictionary<string, GameObject> tokenDict;
    public Dictionary<string, GameObject> cubeDict;

    public Dictionary<string, TokenTemplate> tokenTemplates;
    public Dictionary<string, CubeTemplate> cubeTemplates;

    public GameEnv(){
        tokenDict = new Dictionary<string, GameObject>();
        cubeDict = new Dictionary<string, GameObject>();
        tokenTemplates = new Dictionary<string, TokenTemplate>();
        cubeTemplates = new Dictionary<string, CubeTemplate>();
    }
}