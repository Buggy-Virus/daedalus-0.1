using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube {

	public string graphicAsset;
	public string identifier;

	public type mainType;
	public type[] materialTypes;
	public float[] materialTypesDistribution;
	
	public int state;

	public float transperancy;

	public int viscosity;
	public bool flow;

	public int hitScore;
	public int hitPoints;
    
    public cube (
    	string ga,
    	string id,
    	type t,
    	type[] mt,
    	float[] mtd,
    	int st,
    	float ty,
    	int vy,
    	bool fl,
    	int hs
    	) {
    	graphicAsset = ga;
    	identifier = id;

    	mainType = t;
    	materialTypes = mt;

    	state = st;

    	transperancy = ty;
    	viscosity = vy;
    	flow = fl;

    	hitScore = hs;
    }
}