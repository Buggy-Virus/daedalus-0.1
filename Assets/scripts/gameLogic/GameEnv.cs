using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnv
{
    public TypeTreeScript typeTreeScript;

    public Dictionary<string, Token> tokenDict;
    public Dictionary<string, Cube> cubeDict;

    public GameCoord[,,] gameBoard;
    public Dictionary<Index, GameCoord> indexMap;

    public GameEnv(){
        tokenDict = new Dictionary<string, Token>();
        cubeDict = new Dictionary<string, Cube>();
    }
}