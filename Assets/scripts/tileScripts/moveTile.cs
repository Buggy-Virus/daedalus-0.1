using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveTile {

	public int moveCost;
	public bool accessible;
	public bool visited;
	public moveTile prevTile;

	moveTile() {
		moveCost = int.MaxValue;
		visited = false;
		accessible = true;
	}
}
