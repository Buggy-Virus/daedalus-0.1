using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDT {

    public string tileString;
    public bool passable;

    public TileDT(string n, bool p) {
        tileString = n;
        passable = p;
    }
}
