using System.Collections;
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

    public Dictionary<string, string> variables = new Dictionary<string, string>(); //int=1, float=1, string=3, bool=4
    public Dictionary<string, bool> boolVars = new Dictionary<string, bool>();
    public Dictionary<string, string> stringVars = new Dictionary<string, string>();
    public Dictionary<string, int> intVars = new Dictionary<string, int>();
    public Dictionary<string, double> doubleVars = new Dictionary<string, double>();
    
}
