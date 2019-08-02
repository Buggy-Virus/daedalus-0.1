using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour {
    public GameEnv gameEnv;

    public string identifier;
	public string graphicAsset;
    public GameObject graphicObjectPrefab;
    public GameObject graphicObject;
    public bool onMap = false;

    public Index index;

	public Type type;
	public List<Type> materialTypes;
	public List<float> materialTypesDistribution;

	public float transparency;

	public Dictionary<string, Effect> effects;
    public Dictionary<string, Reffect> reffects;
    public Dictionary<string, Teffect> teffects;

    public Dictionary<string, GameVar> variables = new Dictionary<string, GameVar>();

    public void moveTo(Index endIndex) {
        gameEnv.gameBoard[index.x, index.y, index.z].tokens.Remove(gameObject);
        index = endIndex;
        gameEnv.gameBoard[index.x, index.y, index.z].tokens.Add(gameObject);
        transform.GetComponentInChildren<GraphicCubeScript>().moveTo(new Vector3(endIndex.x, endIndex.y, endIndex.z));
    }
    
}
