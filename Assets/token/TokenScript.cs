using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenScript : MonoBehaviour {
    public GameEnv gameEnv;

    public string identifier;
    public string uid;
    public string graphicAsset;
    public GameObject graphicObjectPrefab;
    public GameObject graphicObject;
    public GameObject tokenPrefab;
    public string alias;

    public Index index;

    public Type type;
	public List<Type> materialTypes;
	public List<float> materialTypesDistribution;
	
    public int width;
    public int length;
    public int height;

    public List<Action> availableActions;
    public List<Raction> availableRactions;
    public List<Taction> availableTactions;

    public Dictionary<string, Effect> effects = new Dictionary<string, Effect>();
    public Dictionary<string, Reffect> reffects = new Dictionary<string, Reffect>();
    public Dictionary<string, Teffect> teffects = new Dictionary<string, Teffect>();

    public Dictionary<string, GameVar> variables = new Dictionary<string, GameVar>();

    public void moveTo(Index endIndex) {
        gameEnv.gameBoard[index.x, index.y, index.z].tokens.Remove(gameObject);
        index = endIndex;
        gameEnv.gameBoard[index.x, index.y, index.z].tokens.Add(gameObject);
        transform.GetComponentInChildren<GraphicTokenScript>().moveTo(new Vector3(endIndex.x, endIndex.y, endIndex.z));
    }
}
