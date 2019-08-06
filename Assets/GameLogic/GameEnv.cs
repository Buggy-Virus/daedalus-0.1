using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnv
{
    public int inGameTime;

    public float cubeSize = 1;

    public MapScript mapScript;

    public GameObject tokensObject;
    public GameObject cubesObject;

    public Dictionary<string, GameObject> tokenDict;
    public Dictionary<string, GameObject> cubeDict;

    public Dictionary<string, Action> actionDict;
    public Dictionary<string, Raction> ractionDict;
    public Dictionary<string, Taction> tactionDict;

    public Dictionary<string, Effect> effectDict;
    public Dictionary<string, Reffect> reffectDict;
    public Dictionary<string, Teffect> teffectDict;

    public Dictionary<string, TokenTemplate> tokenTemplates;
    public Dictionary<string, CubeTemplate> cubeTemplates;

    public GameEnv(){
        tokenDict = new Dictionary<string, GameObject>();
        cubeDict = new Dictionary<string, GameObject>();
        tokenTemplates = new Dictionary<string, TokenTemplate>();
        cubeTemplates = new Dictionary<string, CubeTemplate>();
        actionDict = new Dictionary<string, Action>();
        ractionDict = new Dictionary<string, Raction>();
        tactionDict = new Dictionary<string, Taction>();
        effectDict = new Dictionary<string, Effect>();
        reffectDict = new Dictionary<string, Reffect>();
        teffectDict = new Dictionary<string, Teffect>();
    }
}