using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour {
    public string identifier;
    public string uniqueIdentifier;
    public string graphicAsset;
    public GameObject robject;
    public string name;

    public Index index;

    public Type mainType;
	public List<Type> materialTypes;
	public List<float> materialTypesDistribution;
	
    public int width;
    public int length;
    public int height;

    public int hitScore;
    public int hitPoints;

    public int moveScore;
    public int movePoints;

    public int strength;
    public int dexterity;
    public int endurance;
    public int constitution;
    public int wisdom;
    public int intelligence;

    public Mob(
    	string id,
        string uid,
        string ga,
        Type t,
    	List<Type> mt,
    	List<float> mtd,
    	string nm,
    	int w,
    	int l,
    	int h,
    	int hs,
    	int ms,
    	int str,
    	int dex,
    	int end,
    	int con,
    	int wis,
    	int inl
    	) {
        graphicAsset = ga;

        identifier = id;
        uniqueIdentifier = uid;

        mainType = t;
        materialTypes = mt;
        materialTypesDistribution = mtd;

        name = nm;

        width = w;
        length = l;
        height = h;

        hitScore = hs;
        hitPoints = hs;
        moveScore = ms;
        movePoints = ms;
        
        strength = str;
        dexterity = dex;
        endurance = end;
        constitution = con;
        wisdom = wis;
        intelligence = inl;
    }
}
