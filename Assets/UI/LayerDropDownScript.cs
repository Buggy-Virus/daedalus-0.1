using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerDropDownScript : MonoBehaviour
{
    public MapScript mapScript;

    public void updateValue() {
        List<string> options = new List<string>();
        for (int i = 0; i < mapScript.sizeY; i++) {
            options.Add(i.ToString());
        }
        gameObject.GetComponent<Dropdown>().AddOptions(options);
    }

    void Start() {
        updateValue();
    }
}
