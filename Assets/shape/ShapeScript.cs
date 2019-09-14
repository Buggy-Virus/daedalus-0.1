using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeScript : MonoBehaviour {
    public GameEnv gameEnv;

    public string identifier;
	public string graphicAsset;
    public GameObject graphicObjectPrefab;
    public GameObject graphicObject;
    public GraphicShapeScript graphicShapeScript;
    public bool onMap = false;

    public Index index;

	public Type type;
	public List<Type> materialTypes;
	public List<float> materialTypesDistribution;

	public float transparency;

	public Dictionary<string, Effect> effects;

    public Dictionary<string, GameVar> variables = new Dictionary<string, GameVar>();

    public void moveTo(Index endIndex) {
        gameEnv.mapScript.gameBoard[index.x, index.y, index.z].tokens.Remove(gameObject);
        index = endIndex;
        gameEnv.mapScript.gameBoard[index.x, index.y, index.z].tokens.Add(gameObject);
        graphicShapeScript.moveTo(new Vector3(endIndex.x, endIndex.y, endIndex.z));
    } 
}
