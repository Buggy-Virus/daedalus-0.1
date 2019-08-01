using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenTemplate {
    public GameObject graphicObjectPrefab;
    public string identifier;
    public List<string> aliasList;

    public Type type;
    public List<Type> materialTypes;
    public List<float> materialTypesDistribution;

    public int width;
    public int length;
    public int height;

    public List<Effect> effects;

    public List<Action> availableActions;
    public List<Raction> availableRactions;
    public List<Taction> availableTactions;

    public Dictionary<string, TokenTemplateVariable> initVariableList;

    public TokenTemplate(
        GameObject go,
        string id,
        List<string> al,
        Type t,
        List<Type> mt,
        List<float> mtd,
        int w,
        int l,
        int h,
        List<Effect> e,
        List<Action> aa,
        List<Raction> ar,
        List<Taction> at,
        Dictionary<string, TokenTemplateVariable> ivl
    ) {
        graphicObjectPrefab = go;
        identifier = id;
        aliasList = al;
        type = t;
        materialTypes = mt;
        materialTypesDistribution = mtd;
        width = w;
        length = l;
        height = h;
        effects = e;
        availableActions = aa;
        availableRactions = ar;
        availableTactions = at;
        initVariableList = ivl;
    }
}
