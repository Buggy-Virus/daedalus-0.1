using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeTemplate {
    public GameObject graphicObjectPrefab;
    public string identifier;

    public Type type;
    public List<Type> materialTypes;
    public List<float> materialTypesDistribution;

    public TemplateVariable transparency;

    public List<Effect> effects;

    public Dictionary<string, TemplateVariable> initVariableList;

    public ShapeTemplate(
        GameObject go,
        string id,
        Type t,
        List<Type> mt,
        List<float> mtd,
        TemplateVariable tr,
        List<Effect> e,
        Dictionary<string, TemplateVariable> ivl
    ) {
        graphicObjectPrefab = go;
        identifier = id;
        type = t;
        materialTypes = mt;
        materialTypesDistribution = mtd;
        transparency = tr;
        effects = e;
        initVariableList = ivl;
    }
}
