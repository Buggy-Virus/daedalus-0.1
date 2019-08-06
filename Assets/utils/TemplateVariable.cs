using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateVariable {
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
    public string stringCalculation;
    public string boolCalculation;
    public string intCalculation;
    public string doubleCalculation;

    public TemplateVariable(string x) {
        type = "stringConstant";
        stringConstant = x;
    }

    public TemplateVariable(bool x) {
        type = "boolConstant";
        boolConstant = x;
    }

    public TemplateVariable(int x) {
        type = "intConstant";
        intConstant = x;
    }

    public TemplateVariable(double x) {
        type = "doubleConstant";
        doubleConstant = x; 
    }

    public TemplateVariable(List<string> x) {
        type = "stringList";
        stringList = x;
    }

    public TemplateVariable(List<int> x) {
        type = "intList";
        intList = x;
    }

    public TemplateVariable(List<double> x) {
        type = "doubleList";
        doubleList = x;
    }

    public TemplateVariable(int x, int y) {
        type = "intRange";
        intMin = x;
        intMax = y;
    }

    public TemplateVariable(double x, double y) {
        type = "doubleRange";
        doubleMin = x;
        doubleMax = y;
    }

    public TemplateVariable(double x, bool y) {
        type = "boolProbablity";
        boolProbability = x; 
    }

    public TemplateVariable(string x, string y) {
        type = "stringCalculation";
        stringCalculation = x; 
    }

    public TemplateVariable(string x, bool y) {
        type = "boolCalculation";
        boolCalculation = x; 
    }

    public TemplateVariable(string x, int y) {
        type = "intCalculation";
        intCalculation = x; 
    }

    public TemplateVariable(string x, double y) {
        type = "doubleCalculation";
        doubleCalculation = x; 
    }    
}
