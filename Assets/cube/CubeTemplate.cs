using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTemplate {
    public GameObject graphicObjectPrefab;
    public string identifier;

    public Type type;
    public List<Type> materialTypes;
    public List<float> materialTypesDistribution;

    public int width;
    public int length;
    public int height;

    public List<Effect> effects;

    public Dictionary<string, TemplateVariable> initVariableList;

    public CubeTemplate(
        GameObject go,
        string id,
        Type t,
        List<Type> mt,
        List<float> mtd,
        int w,
        int l,
        int h,
        List<Effect> e,
        Dictionary<string, TemplateVariable> ivl
    ) {
        graphicObjectPrefab = go;
        identifier = id;
        type = t;
        materialTypes = mt;
        materialTypesDistribution = mtd;
        width = w;
        length = l;
        height = h;
        effects = e;
        initVariableList = ivl;
    }
}
