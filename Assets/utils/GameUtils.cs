using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils {

    // ========================================================================================================================== Token Stuff
    static public GameObject quickCreateToken(GameObject tokenPrefab, TokenTemplate template, ref GameEnv gameEnv) {
        System.Random random = new System.Random();

        GameObject token = GameObject.Instantiate(tokenPrefab, gameEnv.tokensObject.transform);
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        tokenScript.gameEnv = gameEnv;
        tokenScript.identifier = template.identifier;
        token.name = template.identifier + "_" + System.Guid.NewGuid();
        gameEnv.tokenDict[token.name] = token;

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

        foreach (KeyValuePair<string, TemplateVariable> var in template.initVariableList) {
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
        return token;
    }

    static public void createGraphicToken(ref GameObject token, Index pos) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        float cubeSize = tokenScript.gameEnv.cubeSize;

        tokenScript.gameEnv.gameBoard[pos.x, pos.y, pos.z].tokens.Add(token);

        Vector3 placePos = new Vector3(pos.x * cubeSize, pos.y * cubeSize, pos.z * cubeSize);

        GameObject graphicToken = GameObject.Instantiate(tokenScript.graphicObjectPrefab, placePos, Quaternion.identity, token.transform);
        tokenScript.graphicObject = graphicToken;
        tokenScript.index = pos;
    }

    static public void createGraphicToken(ref GameObject token, Vector3 pos) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        float cubeSize = tokenScript.gameEnv.cubeSize;

        Index index = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));

        tokenScript.gameEnv.gameBoard[index.x, index.y, index.z].tokens.Add(token);

        Vector3 placePos = new Vector3(index.x * cubeSize, index.y * cubeSize, index.z * cubeSize);

        GameObject graphicToken = GameObject.Instantiate(tokenScript.graphicObjectPrefab, placePos, Quaternion.identity, token.transform);
        tokenScript.graphicObject = graphicToken;
        tokenScript.index = index;
    }

    // ========================================================================================================================== Cube Stuff
    static public GameObject quickCreateCube(GameObject cubePrefab, CubeTemplate template, ref GameEnv gameEnv) {
        System.Random random = new System.Random();

        GameObject cube = GameObject.Instantiate(cubePrefab, gameEnv.cubesObject.transform);
        CubeScript cubeScript = cube.GetComponent<CubeScript>();
        cubeScript.gameEnv = gameEnv;
        cubeScript.identifier = template.identifier;
        cube.name = template.identifier + "_" + System.Guid.NewGuid();
        gameEnv.tokenDict[cube.name] = cube;

        cubeScript.graphicObjectPrefab = template.graphicObjectPrefab;

        cubeScript.type = template.type;
        cubeScript.materialTypes = template.materialTypes;
        cubeScript.materialTypesDistribution = template.materialTypesDistribution;

        foreach (Effect effect in template.effects) {
            if (effect.stacks) {
                effect.givenName = effect.name + "_" + System.Guid.NewGuid();
                
            } else {
                effect.givenName = effect.name;
            }
            cubeScript.effects[effect.givenName] = effect;
        }

        foreach (KeyValuePair<string, TemplateVariable> var in template.initVariableList) {
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
            cubeScript.variables[var.Key] = gameVar;
        }
        return cube;
    }

    static public void createGraphicCube(ref GameObject cube, Index pos) {
        CubeScript cubeScript = cube.GetComponent<CubeScript>();
        float cubeSize = cubeScript.gameEnv.cubeSize;

        cubeScript.gameEnv.gameBoard[pos.x, pos.y, pos.z].cube = cube;

        Vector3 placePos = new Vector3(pos.x * cubeSize, pos.y * cubeSize, pos.z * cubeSize);

        GameObject graphicToken = GameObject.Instantiate(cubeScript.graphicObjectPrefab, placePos, Quaternion.identity, cube.transform);
        cubeScript.graphicObject = graphicToken;
        cubeScript.index = pos;
    }

    static public void createGraphicCube(ref GameObject cube, Vector3 pos) {
        CubeScript cubeScript = cube.GetComponent<CubeScript>();
        float cubeSize = cubeScript.gameEnv.cubeSize;

        Index index = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));

        cubeScript.gameEnv.gameBoard[index.x, index.y, index.z].tokens.Add(cube);

        Vector3 placePos = new Vector3(index.x * cubeSize, index.y * cubeSize, index.z * cubeSize);

        GameObject graphicToken = GameObject.Instantiate(cubeScript.graphicObjectPrefab, placePos, Quaternion.identity, cube.transform);
        cubeScript.graphicObject = graphicToken;
        cubeScript.index = index;
    }

    // ========================================================================================================================== Testing Functions
    static public void createAndPlaceToken(GameObject tokenPrefab, TokenTemplate template, ref GameEnv gameEnv, Index pos) {
        GameObject token = quickCreateToken(tokenPrefab, template, ref gameEnv);
        createGraphicToken(ref token, pos);
    }

    static public void createAndPlaceCube(GameObject cubePrefab, CubeTemplate template, ref GameEnv gameEnv, Index pos) {
        GameObject cube = quickCreateCube(cubePrefab, template, ref gameEnv);
        createGraphicCube(ref cube, pos);
    }
}