using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {

    public string identifier;
    public string uniqueIdentifier;
	public string graphicAsset;
    public GameObject robject;

    public Index index;

	public Type mainType;
	public List<Type> materialTypes;
	public List<float> materialTypesDistribution;
	
	public int state;

	public float transparency;

	public int viscosity;
	public bool flow;

	public int hitScore;
	public int hitPoints;
    
    public Cube (
        string id,
        string uid,
    	string ga,
    	Type t,
    	List<Type> mt,
    	List<float> mtd,
    	int st,
    	float ty,
    	int vy,
    	bool fl,
    	int hs
    	) {
    	graphicAsset = ga;
        uniqueIdentifier = uid;
    	identifier = id;

    	mainType = t;
    	materialTypes = mt;

    	state = st;

    	transparency = ty;
    	viscosity = vy;
    	flow = fl;

    	hitScore = hs;
    }
}