using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeTreeScript : MonoBehaviour
{
    
	public Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

    void Start()
    {
    	typeDict.Add("cube", new Type("cube"));

        typeDict.Add("token", new Type("token"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
