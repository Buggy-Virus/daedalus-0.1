using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils {

    static void quickCreateToken(GameObject tokenPrefab, TokenTemplate template, GameObject tokensObject, Dictionary<string, GameObject> tokenDict) {
        System.Random random = new System.Random();

        GameObject token = GameObject.Instantiate(tokenPrefab, tokensObject.transform);
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        tokenScript.identifier = template.identifier;
        token.name = template.identifier + "_" + System.Guid.NewGuid();
        tokenDict[token.name] = token;

        tokenScript.graphicObjectPrefab = template.graphicObjectPrefab;

        tokenScript.alias = template.aliasList[random.Next(template.aliasList.Count)];

        tokenScript.type = template.type;
        tokenScript.materialTypes = template.materialTypes;
        tokenScript.materialTypesDistribution = template.materialTypesDistribution;

        tokenScript.width = template.width;
        tokenScript.length = template.length;
        tokenScript.height = template.height;

        foreach (Effect effect in template.effects) {
            if (effect.stacks) {
                effect.givenName = effect.name + "_" + System.Guid.NewGuid();
                
            } else {
                effect.givenName = effect.name;
            }
            tokenScript.effects[effect.givenName] = effect;
        }

        tokenScript.availableActions = template.availableActions;
        tokenScript.availableRactions = template.availableRactions;
        tokenScript.availableTactions = template.availableTactions;

        foreach (KeyValuePair<string, TokenTemplateVariable> var in template.initVariableList) {
            GameVar gameVar = new GameVar();
            switch(var.Value.type) {
                case "stringConstant":
                    gameVar.type = "string";
                    gameVar.stringValue = var.Value.stringConstant;
                    break;
                case "boolConstant":
                    gameVar.type = "bool";
                    gameVar.boolValue = var.Value.boolConstant;
                    break;
                case "intConstant":
                    gameVar.type = "int";
                    gameVar.intValue = var.Value.intConstant;
                    break;
                case "doubleConstant":
                    gameVar.type = "double";
                    gameVar.doubleValue = var.Value.doubleConstant;
                    break;
                case "stringList":
                    gameVar.type = "string";
                    gameVar.stringValue = var.Value.stringList[random.Next(var.Value.stringList.Count)];
                    break;
                case "intList":
                    gameVar.type = "int";
                    gameVar.intValue = var.Value.intList[random.Next(var.Value.intList.Count)];
                    break;
                case "doubleList":
                    gameVar.type = "double";
                    gameVar.doubleValue = var.Value.doubleList[random.Next(var.Value.doubleList.Count)];
                    break;
                case "intRange":
                    gameVar.type = "int";
                    gameVar.intValue = random.Next(var.Value.intMin, var.Value.intMax);
                    break;
                case "doubleRange":
                    gameVar.type = "double";
                    gameVar.doubleValue = var.Value.doubleMin + (var.Value.doubleMax - var.Value.doubleMin) * random.NextDouble();
                    break;
                case "boolProbablity":
                    gameVar.type = "bool";
                    gameVar.boolValue = (random.NextDouble() < var.Value.boolProbability);
                    break;
            }
            tokenScript.variables[var.Key] = gameVar;
        }
    }

    static void createGraphicToken(GameObject token, Index pos, float cubeSize) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();

        Vector3 placePos = new Vector3(pos.x * cubeSize, pos.y * cubeSize, pos.z * cubeSize);

        GameObject graphicToken = GameObject.Instantiate(tokenScript.graphicObjectPrefab, placePos, Quaternion.identity, token.transform);
        tokenScript.graphicObject = graphicToken;
        tokenScript.index = pos;
    }

    static void createGraphicToken(GameObject token, Vector3 pos, float cubeSize) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();

        Index index = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));

        Vector3 placePos = new Vector3(index.x * cubeSize, index.y * cubeSize, index.z * cubeSize);

        GameObject graphicToken = GameObject.Instantiate(tokenScript.graphicObjectPrefab, placePos, Quaternion.identity, token.transform);
        tokenScript.graphicObject = graphicToken;
        tokenScript.index = index;
    }
}