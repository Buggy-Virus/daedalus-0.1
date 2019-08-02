using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateVariable {
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

    TemplateVariable(string n, string x) {
        variableName = n;
        type = "stringConstant";
        stringConstant = x;
    }

    TemplateVariable(string n, bool x) {
        variableName = n;
        type = "boolConstant";
        boolConstant = x;
    }

    TemplateVariable(string n, int x) {
        variableName = n;
        type = "intConstant";
        intConstant = x;
    }

    TemplateVariable(string n, double x) {
        variableName = n;
        type = "doubleConstant";
        doubleConstant = x; 
    }

    TemplateVariable(string n, List<string> x) {
        variableName = n;
        type = "stringList";
        stringList = x;
    }

    TemplateVariable(string n, List<int> x) {
        variableName = n;
        type = "intList";
        intList = x;
    }

    TemplateVariable(string n, List<double> x) {
        variableName = n;
        type = "doubleList";
        doubleList = x;
    }

    TemplateVariable(string n, int x, int y) {
        variableName = n;
        type = "intRange";
        intMin = x;
        intMax = y;
    }

    TemplateVariable(string n, double x, double y) {
        variableName = n;
        type = "doubleRange";
        doubleMin = x;
        doubleMax = y;
    }

    TemplateVariable(string n, double x, bool y) {
        variableName = n;
        type = "boolProbablity";
        boolProbability = x; 
    }
}
