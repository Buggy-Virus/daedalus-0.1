using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenTemplateVariable {
    public string variableName;
	public string type;
    public string stringConstant;
    public bool boolConstant;
    public int intConstant;
    public double doubleConstant;
    public List<string> stringList;
    public List<int> intList;
    public List<double> doubleList;
    public int intMin;
    public int intMax;
    public double doubleMin;
    public double doubleMax;
    public double boolProbability;

    TokenTemplateVariable(string n, string x) {
        variableName = n;
        type = "stringConstant";
        stringConstant = x;
    }

    TokenTemplateVariable(string n, bool x) {
        variableName = n;
        type = "boolConstant";
        boolConstant = x;
    }

    TokenTemplateVariable(string n, int x) {
        variableName = n;
        type = "intConstant";
        intConstant = x;
    }

    TokenTemplateVariable(string n, double x) {
        variableName = n;
        type = "doubleConstant";
        doubleConstant = x; 
    }

    TokenTemplateVariable(string n, List<string> x) {
        variableName = n;
        type = "stringList";
        stringList = x;
    }

    TokenTemplateVariable(string n, List<int> x) {
        variableName = n;
        type = "intList";
        intList = x;
    }

    TokenTemplateVariable(string n, List<double> x) {
        variableName = n;
        type = "doubleList";
        doubleList = x;
    }

    TokenTemplateVariable(string n, int x, int y) {
        variableName = n;
        type = "intRange";
        intMin = x;
        intMax = y;
    }

    TokenTemplateVariable(string n, double x, double y) {
        variableName = n;
        type = "doubleRange";
        doubleMin = x;
        doubleMax = y;
    }

    TokenTemplateVariable(string n, double x, bool y) {
        variableName = n;
        type = "boolProbablity";
        boolProbability = x; 
    }
}
