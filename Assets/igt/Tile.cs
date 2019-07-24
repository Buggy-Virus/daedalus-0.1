using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public string graphicAsset;
	public string identifier;

	public Type mainType;
	public Type[] materialTypes;
	public float[] materialTypesDistribution;

	public float transperancy;

	public int hitScore;
    
    public Tile (
    	string ga,
    	string id,
    	Type[] mt,
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