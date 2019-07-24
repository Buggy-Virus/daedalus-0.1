using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCoord : MonoBehaviour {

	public int x;
	public int y;
	public int z;

	public Cube cube;
	public Cube upCube;
	public Cube downCube;
	public List<Token> tokens;
	public List<Character> characters;
	public Tile downTile;
	public Tile upTile;

    
    public GameCoord (int inpX, int inpY, int inpZ) {
    	x = inpX;
    	y = inpY;
    	z = inpZ;
    }
}