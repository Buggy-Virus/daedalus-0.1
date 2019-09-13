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
    public GameObject shapesObject;

    public Dictionary<string, GameObject> tokenDict;
    public Dictionary<string, GameObject> shapeDict;
    public Dictionary<string, GameObject> wallDict;

    public Dictionary<string, Action> actionDict;
    public Dictionary<string, Effect> effectDict;

    public Dictionary<string, TokenTemplate> tokenTemplates;
    public Dictionary<string, ShapeTemplate> shapeTemplates;
    public Dictionary<string, WallTemplate> wallTemplates;

    public GameEnv(){
        tokenDict = new Dictionary<string, GameObject>();
        shapeDict = new Dictionary<string, GameObject>();
        wallDict = new Dictionary<string, GameObject>();
        tokenTemplates = new Dictionary<string, TokenTemplate>();
        shapeTemplates = new Dictionary<string, ShapeTemplate>();
        wallTemplates = new Dictionary<string, WallTemplate>();
        actionDict = new Dictionary<string, Action>();
        effectDict = new Dictionary<string, Effect>();
    }
}