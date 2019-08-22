using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils {
    
    static GameVar parseTemplateVariable(TemplateVariable var, ref GameEnv gameEnv) {
        System.Random random = new System.Random();
        Value result;
        switch(var.type) {
            case "stringConstant":
                return new GameVar(var.stringConstant);
            case "boolConstant":
                return new GameVar(var.boolConstant);
            case "intConstant":
                return new GameVar(var.intConstant);
            case "doubleConstant":
                return new GameVar(var.doubleConstant);
            case "stringList":
                return new GameVar(var.stringList[random.Next(var.stringList.Count)]);
            case "intList":
                return new GameVar(var.intList[random.Next(var.intList.Count)]);
            case "doubleList":
                return new GameVar(var.doubleList[random.Next(var.doubleList.Count)]);
            case "intRange":
                return new GameVar(random.Next(var.intMin, var.intMax));
            case "doubleRange":
                return new GameVar(var.doubleMin + (var.doubleMax - var.doubleMin) * random.NextDouble());
            case "boolProbablity":
                return new GameVar(random.NextDouble() < var.boolProbability);
            case "stringCalculation":
                result = DaedScript.evaluate(var.stringCalculation, ref gameEnv);
                if (result.valueType == "string") {
                    return new GameVar(result.vString);
                } else {
                    return new GameVar("Expected string, got: " + result.valueType);
                }
            case "intCalculation":
                result = DaedScript.evaluate(var.intCalculation, ref gameEnv);
                if (result.valueType == "int") {
                    return new GameVar(result.vInt);
                } else {
                    return new GameVar("Expected int, got: " + result.valueType);
                }
            case "doubleCalculation":
                result = DaedScript.evaluate(var.doubleCalculation, ref gameEnv);
                if (result.valueType == "double") {
                    return new GameVar(result.vString);
                } else {
                    return new GameVar("Expected double, got: " + result.valueType);
                }
            case "boolCalculation":
                result = DaedScript.evaluate(var.boolCalculation, ref gameEnv);
                if (result.valueType == "bool") {
                    return new GameVar(result.vString);
                } else {
                    return new GameVar("Expected bool, got: " + result.valueType);
                }
            default:
                return new GameVar();
        }
    }

    // ========================================================================================================================== Token Stuff
    static public GameObject quickCreateToken(GameObject tokenPrefab, TokenTemplate template, ref GameEnv gameEnv) {
        GameObject token = GameObject.Instantiate(tokenPrefab, gameEnv.tokensObject.transform);
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        tokenScript.gameEnv = gameEnv;
        tokenScript.identifier = template.identifier;
        token.name = template.identifier + "_" + System.Guid.NewGuid();
        gameEnv.tokenDict[token.name] = token;

        tokenScript.graphicObjectPrefab = template.graphicObjectPrefab;

        GameVar alias_calculation = parseTemplateVariable(template.aliasList, ref gameEnv);
        if (alias_calculation.type == "string") {
            tokenScript.alias = alias_calculation.stringValue;
        } else {
            // Throw Error
        } 

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

        tokenScript.actions = new Dictionary<string, Action>();
        foreach (Action action in template.availableActions) {
            tokenScript.actions.Add(action.name, action);
        }
        
        foreach (KeyValuePair<string, TemplateVariable> var in template.initVariableList) {
            tokenScript.variables[var.Key] = parseTemplateVariable(var.Value, ref gameEnv);
        }

        GameObject graphicToken = GameObject.Instantiate(tokenScript.graphicObjectPrefab, token.transform);
        tokenScript.graphicObject = graphicToken;
        graphicToken.GetComponent<GraphicTokenScript>().token = token;
        graphicToken.GetComponent<GraphicTokenScript>().tokenScript = tokenScript;
        graphicToken.SetActive(false);

        return token;
    }

    static public void placeGraphicToken(ref GameObject token, Index pos) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        tokenScript.onMap = true;
        float cubeSize = tokenScript.gameEnv.cubeSize;

        tokenScript.gameEnv.mapScript.gameBoard[pos.x, pos.y, pos.z].tokens.Add(token);
        tokenScript.index = pos;

        Vector3 placePos = new Vector3(pos.x * cubeSize, pos.y * cubeSize, pos.z * cubeSize);

        tokenScript.graphicObject.SetActive(true);
        tokenScript.graphicObject.transform.position = placePos;  
        tokenScript.graphicObject.GetComponent<GraphicTokenScript>().goingTo = placePos;     
    }

    static public void placeGraphicToken(ref GameObject token, Vector3 pos) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        tokenScript.onMap = true;
        float cubeSize = tokenScript.gameEnv.cubeSize;

        Index index = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));

        tokenScript.gameEnv.mapScript.gameBoard[index.x, index.y, index.z].tokens.Add(token);
        tokenScript.index = index;

        Vector3 placePos = new Vector3(index.x * cubeSize, index.y * cubeSize, index.z * cubeSize);

        tokenScript.graphicObject.SetActive(true);
        tokenScript.graphicObject.transform.position = placePos; 
        tokenScript.graphicObject.GetComponent<GraphicTokenScript>().goingTo = placePos;   
    }

    static public void deleteGraphicToken(ref GameObject token) {
        CubeScript tokenScript = token.GetComponent<CubeScript>();
        Index index = tokenScript.index;
        tokenScript.gameEnv.mapScript.gameBoard[index.x, index.y, index.z].tokens.Remove(token);
        tokenScript.graphicObject.SetActive(false);
        tokenScript.onMap = false;
    }

    static public void deleteToken(ref GameObject token) {
        CubeScript tokenScript = token.GetComponent<CubeScript>();
        if (tokenScript.onMap) {
            deleteGraphicCube(ref token);
        }
        GameObject.Destroy(token);
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

        GameVar transparency_calculation = parseTemplateVariable(template.transparency, ref gameEnv);
        if (transparency_calculation.type == "double") {
            cubeScript.transparency = (float)transparency_calculation.doubleValue;
        } else {
            // Throw Error
        } 

        foreach (Effect effect in template.effects) {
            if (effect.stacks) {
                effect.givenName = effect.name + "_" + System.Guid.NewGuid();
                
            } else {
                effect.givenName = effect.name;
            }
            cubeScript.effects[effect.givenName] = effect;
        }

        foreach (KeyValuePair<string, TemplateVariable> var in template.initVariableList) {
            cubeScript.variables[var.Key] = parseTemplateVariable(var.Value, ref gameEnv);
        }

        GameObject graphicCube = GameObject.Instantiate(cubeScript.graphicObjectPrefab, cube.transform);
        cubeScript.graphicObject = graphicCube;
        graphicCube.GetComponent<GraphicCubeScript>().cube = cube;
        graphicCube.GetComponent<GraphicCubeScript>().cubeScript = cubeScript;
        graphicCube.SetActive(false);

        return cube;
    }

    static public void placeGraphicCube(ref GameObject cube, Index pos) {
        CubeScript cubeScript = cube.GetComponent<CubeScript>();
        cubeScript.onMap = true;
        float cubeSize = cubeScript.gameEnv.cubeSize;

        cubeScript.gameEnv.mapScript.gameBoard[pos.x, pos.y, pos.z].cube = cube;
        cubeScript.index = pos;

        Vector3 placePos = new Vector3(pos.x * cubeSize, pos.y * cubeSize, pos.z * cubeSize);
        cubeScript.graphicObject.SetActive(true);
        cubeScript.graphicObject.transform.position = placePos;
        cubeScript.graphicObject.GetComponent<GraphicCubeScript>().goingTo = placePos;
    }

    static public void placeGraphicCube(ref GameObject cube, Vector3 pos) {
        CubeScript cubeScript = cube.GetComponent<CubeScript>();
        cubeScript.onMap = true;
        float cubeSize = cubeScript.gameEnv.cubeSize;

        Index index = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));

        cubeScript.gameEnv.mapScript.gameBoard[index.x, index.y, index.z].cube = cube;
        cubeScript.index = index;

        Vector3 placePos = new Vector3(index.x * cubeSize, index.y * cubeSize, index.z * cubeSize);
        cubeScript.graphicObject.SetActive(true);
        cubeScript.graphicObject.transform.position = placePos;
        cubeScript.graphicObject.GetComponent<GraphicCubeScript>().goingTo = placePos;
    }

    static public void deleteGraphicCube(ref GameObject cube) {
        CubeScript cubeScript = cube.GetComponent<CubeScript>();
        Index index = cubeScript.index;
        cubeScript.gameEnv.mapScript.gameBoard[index.x, index.y, index.z].cube = null;
        cubeScript.graphicObject.SetActive(false);
        cubeScript.onMap = false;
    }

    static public void deleteCube(ref GameObject cube) {
        CubeScript cubeScript = cube.GetComponent<CubeScript>();
        if (cubeScript.onMap) {
            deleteGraphicCube(ref cube);
        }
        GameObject.Destroy(cube);
    }

    // ========================================================================================================================== Testing Functions
    static public void createAndPlaceToken(GameObject tokenPrefab, TokenTemplate template, ref GameEnv gameEnv, Index pos) {
        GameObject token = quickCreateToken(tokenPrefab, template, ref gameEnv);
        placeGraphicToken(ref token, pos);
    }

    static public void createAndPlaceCube(GameObject cubePrefab, CubeTemplate template, ref GameEnv gameEnv, Index pos) {
        GameObject cube = quickCreateCube(cubePrefab, template, ref gameEnv);
        placeGraphicCube(ref cube, pos);
    }
}