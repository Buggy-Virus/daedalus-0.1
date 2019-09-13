using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    public GameEnv gameEnv;

    public string identifier;
	public string graphicAsset;
    public GameObject graphicObjectPrefab;
    public GameObject graphicObject;
    public bool onMap = false;

    public Index index_xx;
    public Index index_x;
    public Index index_zz;
    public Index index_z;

	public Type type;
	public List<Type> materialTypes;
	public List<float> materialTypesDistribution;

	public float transparency;

	public Dictionary<string, Effect> effects;

    public Dictionary<string, GameVar> variables = new Dictionary<string, GameVar>();

    // public void moveTo(Index endIndex_1, endIndex_2) {
    //     gameEnv.mapScript.gameBoard[index.x, index.y, index.z].tokens.Remove(gameObject);
    //     index = endIndex;
    //     gameEnv.mapScript.gameBoard[index.x, index.y, index.z].tokens.Add(gameObject);
    //     transform.GetComponentInChildren<GraphicShapeScript>().moveTo(new Vector3(endIndex.x, endIndex.y, endIndex.z));
    // }
}
