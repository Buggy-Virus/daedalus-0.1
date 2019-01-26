using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token {

    public TokenDT tokenDT;
    public string name;
    public bool rendered;
    public int moveScore;
    public int movePoints;
    public GameTile curTile;

    public Token(TokenDT tDT, string n, int m, GameTile t) {
        tokenDT = tDT;
        name = n;
        moveScore = m;
        movePoints = m;
        curTile = t;
    }
}
