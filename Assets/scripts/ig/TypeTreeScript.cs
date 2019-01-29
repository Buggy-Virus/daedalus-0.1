using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeTreeScript : MonoBehaviour
{
    
	public Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

    void Start()
    {
    	typeDict.Add("cube", new Type("cube"));

        typeDict.Add("mob", new Type("mob"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
