using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile {

    public TileDT tileDT;
    public int tileType;
    public string tileName;
    public bool rendered;
    public int x;
    public int y;
    public int z;
    public string coord;
    public List<Thing> things;
    public List<Token> tokens;

    public GameTile(TileDT tDT, int x, int y, int z) {
        tileDT = tDT;
        tileType = 0;
        rendered = false;
        things = new List<Thing>();
        tokens = new List<Token>();
        coord = x.ToString() + " " + y.ToString() + " " + z.ToString();
        tileName = tDT.tileString + " " + coord;
    }
}
