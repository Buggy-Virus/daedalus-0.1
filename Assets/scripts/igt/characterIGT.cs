using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character {

    public string graphicAsset;

    public string identifier;
    public string name;

    public type mainType;
	public type[] materialTypes;
	public float[] materialTypesDistribution;

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

    public character(
    	string ga,
    	string id,
    	string nm,
    	type[] mt,
    	float[] mtd,
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
    	int intl
    	) {
        graphicAsset = ga;

        identifier = id;
        name = nm;

        width = w;
        length = l;
        height = h;

        hitScore = hs;
        moveScore = ms;

        strength = str;
        dexterity = dex;
        endurance = end;
        constitution = con;
        wisdom = wis;
        intelligence = intl;
    }
}
