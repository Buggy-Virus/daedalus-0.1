using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Index {
	public string name;

	public int x;
	public int y;
	public int z;

	public Index (int inpX, int inpY, int inpZ) {
		x = inpX;
		y = inpY;
		z = inpZ;
	}

	public Index (Index index) {
		x = index.x;
		y = index.y;
		z = index.z;
	}
}

