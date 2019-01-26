using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile {

	public string graphicAsset;
	public string identifier;

	public type mainType;
	public type[] materialTypes;
	public float[] materialTypesDistribution;

	public float transperancy;

	public int hitScore;
    
    public tile (
    	string ga,
    	string id,
    	type[] mt,
    	float[] mtd,
    	float ty,
    	int hs
    	) {
    	graphicAsset = ga;
    	identifier = id;

    	transperancy = ty;

    	hitScore = hs;
    }
}