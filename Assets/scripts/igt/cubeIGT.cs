﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {

    public string identifier;
    public string uniqueIdentifier;
	public string graphicAsset;
    public GameObject gameObject;

    public Index index;

	public Type type;
	public List<Type> materialTypes;
	public List<float> materialTypesDistribution;
	
	public int state;

	public float transparency;

	public int viscosity;
	public bool flow;

	public int hitScore;
	public int hitPoints;

    public Dictionary<string, bool> boolVars = new Dictionary<string, bool>();
    public Dictionary<string, string> stringVars = new Dictionary<string, string>();
    public Dictionary<string, int> intVars = new Dictionary<string, int>();
    public Dictionary<string, float> floatVars = new Dictionary<string, float>();
    
}