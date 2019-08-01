using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVar {
    public string type;
    public string stringValue;
    public bool boolValue;
    public int intValue;
    public double doubleValue;

    public GameVar() {}

    public GameVar(string x) {
        type = "string";
        stringValue = x;
    }

    public GameVar(bool x) {
        type = "bool";
        boolValue = x;
    }

    public GameVar(int x) {
        type = "int";
        intValue = x;
    }

    public GameVar(double x) {
        type = "double";
        doubleValue = x;
    }
}
