using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils {
    
    static Value parseTemplateVariable(TemplateVariable var, ref GameEnv gameEnv) {
        System.Random random = new System.Random();
        Value result;
        switch(var.type) {
            case "stringConstant":
                return new Value(var.stringConstant, "");
            case "boolConstant":
                return new Value(var.boolConstant);
            case "intConstant":
                return new Value(var.intConstant);
            case "doubleConstant":
                return new Value(var.doubleConstant);
            case "stringList":
                return new Value(var.stringList[random.Next(var.stringList.Count)], "");
            case "intList":
                return new Value(var.intList[random.Next(var.intList.Count)]);
            case "doubleList":
                return new Value(var.doubleList[random.Next(var.doubleList.Count)]);
            case "intRange":
                return new Value(random.Next(var.intMin, var.intMax));
            case "doubleRange":
                return new Value(var.doubleMin + (var.doubleMax - var.doubleMin) * random.NextDouble());
            case "boolProbablity":
                return new Value(random.NextDouble() < var.boolProbability);
            case "stringCalculation":
                result = DaedScript.evaluate(var.stringCalculation, ref gameEnv);
                if (result.valueType == "string") {
                    return new Value(result.vString, "");
                } else {
                    return new Value("Expected string, got: " + result.valueType, false);
                }
            case "intCalculation":
                result = DaedScript.evaluate(var.intCalculation, ref gameEnv);
                if (result.valueType == "int") {
                    return new Value(result.vInt);
                } else {
                    return new Value("Expected int, got: " + result.valueType, false);
                }
            case "doubleCalculation":
                result = DaedScript.evaluate(var.doubleCalculation, ref gameEnv);
                if (result.valueType == "double") {
                    return new Value(result.vString);
                } else {
                    return new Value("Expected double, got: " + result.valueType, false);
                }
            case "boolCalculation":
                result = DaedScript.evaluate(var.boolCalculation, ref gameEnv);
                if (result.valueType == "bool") {
                    return new Value(result.vString);
                } else {
                    return new Value("Expected bool, got: " + result.valueType, false);
                }
            default:
                return new Value();
        }
    }

    // ========================================================================================================================== Token Stuff
    static public GameObject quickCreateToken(GameObject tokenPrefab, TokenTemplate template, ref GameEnv gameEnv) {
        GameObject token = GameObject.Instantiate(tokenPrefab, gameEnv.tokensObject.transform);
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        GraphicTokenScript graphicTokenScript = token.GetComponent<GraphicTokenScript>();
        tokenScript.gameEnv = gameEnv;
        tokenScript.identifier = template.identifier;
        token.name = template.identifier + "_" + System.Guid.NewGuid();
        gameEnv.tokenDict[token.name] = token;

        tokenScript.graphicObjectPrefab = template.graphicObjectPrefab;

        Value alias_calculation = parseTemplateVariable(template.aliasList, ref gameEnv);
        if (alias_calculation.valueType == "string") {
            tokenScript.alias = alias_calculation.vString;
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

        GameObject graphicToken = GameObject.Instantiate(tokenScript.graphicObjectPrefab, token.transform, false);
        // graphicToken.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        graphicToken.SetActive(false);
        graphicTokenScript.token = token;
        graphicTokenScript.tokenScript = tokenScript;
        graphicTokenScript.graphicObject_transform = graphicToken.transform;
        graphicTokenScript.graphicObject_collider = graphicToken.transform.GetComponent<Collider>();
        tokenScript.graphicObject = graphicToken;
        tokenScript.graphicTokenScript = graphicTokenScript;

        GameObject tokenCavas = GameObject.Instantiate(graphicTokenScript.canvasPrefab, token.transform);
        graphicTokenScript.canvas = tokenCavas;

        graphicTokenScript.enabled = false;

        return token;
    }

    static public GameObject quickCreateToken(GameObject tokenPrefab, TokenTemplate template, ref GameEnv gameEnv, string name) {
        GameObject token = GameObject.Instantiate(tokenPrefab, gameEnv.tokensObject.transform);
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        GraphicTokenScript graphicTokenScript = token.GetComponent<GraphicTokenScript>();
        tokenScript.gameEnv = gameEnv;
        tokenScript.identifier = template.identifier;
        token.name = name;
        gameEnv.tokenDict[name] = token;

        tokenScript.graphicObjectPrefab = template.graphicObjectPrefab;

        Value alias_calculation = parseTemplateVariable(template.aliasList, ref gameEnv);
        if (alias_calculation.valueType == "string") {
            tokenScript.alias = alias_calculation.vString;
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

        GameObject graphicToken = GameObject.Instantiate(tokenScript.graphicObjectPrefab, token.transform, false);
        // graphicToken.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        graphicToken.SetActive(false);
        graphicTokenScript.token = token;
        graphicTokenScript.tokenScript = tokenScript;
        graphicTokenScript.graphicObject_transform = graphicToken.transform;
        graphicTokenScript.graphicObject_collider = graphicToken.transform.GetComponent<Collider>();
        tokenScript.graphicObject = graphicToken;
        tokenScript.graphicTokenScript = graphicTokenScript;

        GameObject tokenCavas = GameObject.Instantiate(graphicTokenScript.canvasPrefab, token.transform);
        graphicTokenScript.canvas = tokenCavas;

        graphicTokenScript.enabled = false;

        return token;
    }

    static public void placeGraphicToken(ref GameObject token, Index pos) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        GraphicTokenScript graphicTokenScript = token.GetComponent<GraphicTokenScript>();
        tokenScript.onMap = true;
        float cubeSize = tokenScript.gameEnv.cubeSize;

        tokenScript.gameEnv.mapScript.gameBoard[pos.x, pos.y, pos.z].tokens.Add(token);
        tokenScript.index = pos;

        Vector3 placePos = new Vector3(pos.x * cubeSize, pos.y * cubeSize, pos.z * cubeSize);

        tokenScript.graphicObject.SetActive(true);
        token.transform.position = placePos;  
        
        graphicTokenScript.enabled = true;
        graphicTokenScript.goingTo = placePos;     
    }

    static public void placeGraphicToken(ref GameObject token, Vector3 pos) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        GraphicTokenScript graphicTokenScript = token.GetComponent<GraphicTokenScript>();
        tokenScript.onMap = true;
        float cubeSize = tokenScript.gameEnv.cubeSize;

        Index index = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));

        tokenScript.gameEnv.mapScript.gameBoard[index.x, index.y, index.z].tokens.Add(token);
        tokenScript.index = index;

        Vector3 placePos = new Vector3(index.x * cubeSize, index.y * cubeSize, index.z * cubeSize);

        tokenScript.graphicObject.SetActive(true);
        token.transform.position = placePos; 
        
        graphicTokenScript.enabled = true;
        graphicTokenScript.goingTo = placePos;   
    }

    static public void deleteGraphicToken(ref GameObject token) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        Index index = tokenScript.index;
        tokenScript.gameEnv.mapScript.gameBoard[index.x, index.y, index.z].tokens.Remove(token);
        tokenScript.graphicObject.SetActive(false);
        tokenScript.onMap = false;
        token.GetComponent<GraphicTokenScript>().enabled = false;
    }

    static public void deleteToken(ref GameObject token) {
        TokenScript tokenScript = token.GetComponent<TokenScript>();
        tokenScript.gameEnv.tokenDict.Remove(token.name);
        if (tokenScript.onMap) {
            deleteGraphicToken(ref token);
        }
        GameObject.Destroy(token);
    }

    // ========================================================================================================================== Shape Stuff
    static public GameObject quickCreateShape(GameObject shapePrefab, ShapeTemplate template, ref GameEnv gameEnv) {
        GameObject shape = GameObject.Instantiate(shapePrefab, gameEnv.shapesObject.transform);
        ShapeScript shapeScript = shape.GetComponent<ShapeScript>();
        GraphicShapeScript graphicShapeScript = shape.GetComponent<GraphicShapeScript>();
        shapeScript.gameEnv = gameEnv;
        shapeScript.identifier = template.identifier;
        shape.name = template.identifier + "_" + System.Guid.NewGuid();
        gameEnv.shapeDict[shape.name] = shape;

        shapeScript.graphicObjectPrefab = template.graphicObjectPrefab;

        shapeScript.type = template.type;
        shapeScript.materialTypes = template.materialTypes;
        shapeScript.materialTypesDistribution = template.materialTypesDistribution;

        Value transparency_calculation = parseTemplateVariable(template.transparency, ref gameEnv);
        if (transparency_calculation.valueType == "double") {
            shapeScript.transparency = (float)transparency_calculation.vDouble;
        } else {
            // Throw Error
        } 

        foreach (Effect effect in template.effects) {
            if (effect.stacks) {
                effect.givenName = effect.name + "_" + System.Guid.NewGuid();
                
            } else {
                effect.givenName = effect.name;
            }
            shapeScript.effects[effect.givenName] = effect;
        }

        foreach (KeyValuePair<string, TemplateVariable> var in template.initVariableList) {
            shapeScript.variables[var.Key] = parseTemplateVariable(var.Value, ref gameEnv);
        }

        GameObject graphicShape = GameObject.Instantiate(shapeScript.graphicObjectPrefab, shape.transform, false);
        // graphicShape.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        graphicShape.SetActive(false);
        graphicShapeScript.shapeScript = shapeScript;
        graphicShapeScript.graphicObject_transform = graphicShape.transform;
        graphicShapeScript.graphicObject_collider = graphicShape.transform.GetComponent<Collider>();
        shapeScript.graphicObject = graphicShape;
        shapeScript.graphicShapeScript = graphicShapeScript;
        graphicShapeScript.enabled = false;

        return shape;
    }

    static public void placeGraphicShape(ref GameObject shape, Index pos) {
        ShapeScript shapceScript = shape.GetComponent<ShapeScript>();
        GraphicShapeScript graphicShapeScript = shape.GetComponent<GraphicShapeScript>();
        shapceScript.onMap = true;
        float cubeSize = shapceScript.gameEnv.cubeSize;

        shapceScript.gameEnv.mapScript.gameBoard[pos.x, pos.y, pos.z].shape = shape;
        shapceScript.index = pos;

        Vector3 placePos = new Vector3(pos.x * cubeSize, pos.y * cubeSize, pos.z * cubeSize);
        shapceScript.graphicObject.SetActive(true);
        shape.transform.position = placePos;
        graphicShapeScript.enabled = true;
        graphicShapeScript.goingTo = placePos;
    }

    static public void placeGraphicShape(ref GameObject shape, Vector3 pos) {
        ShapeScript shapeScript = shape.GetComponent<ShapeScript>();
        GraphicShapeScript graphicShapeScript = shape.GetComponent<GraphicShapeScript>();
        shapeScript.onMap = true;
        float cubeSize = shapeScript.gameEnv.cubeSize;

        Index index = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));

        shapeScript.gameEnv.mapScript.gameBoard[index.x, index.y, index.z].shape = shape;
        shapeScript.index = index;

        Vector3 placePos = new Vector3(index.x * cubeSize, index.y * cubeSize, index.z * cubeSize);
        shapeScript.graphicObject.SetActive(true);
        graphicShapeScript.enabled = true;
        graphicShapeScript.goingTo = placePos;
    }

    static public void deleteGraphicShape(ref GameObject shape) {
        ShapeScript shapeScript = shape.GetComponent<ShapeScript>();
        Index index = shapeScript.index;
        shapeScript.gameEnv.mapScript.gameBoard[index.x, index.y, index.z].shape = null;
        shapeScript.graphicObject.SetActive(false);
        shapeScript.onMap = false;
        shape.GetComponent<GraphicShapeScript>().enabled = false;
    }

    static public void deleteShape(ref GameObject shape) {
        ShapeScript shapeScript = shape.GetComponent<ShapeScript>();
        if (shapeScript.onMap) {
            deleteGraphicShape(ref shape);
        }
        GameObject.Destroy(shape);
    }

    static public void deleteShapeAtPos(Vector3 pos, ref MapScript mapScript) {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        GameObject shape = mapScript.gameBoard[x,y,z].shape;
        mapScript.gameBoard[x,y,z].shape = null;
        GameObject.Destroy(shape);
    }

    // ==========================================================================================================================
    // ==========================================================================================================================
    // ==========================================================================================================================
    // ==========================================================================================================================
    // ========================================================================================================================== Wall Stuff
    static public GameObject quickCreateWall(GameObject wallPrefab, WallTemplate template, ref GameEnv gameEnv) {
        GameObject wall = GameObject.Instantiate(wallPrefab, gameEnv.shapesObject.transform);
        WallScript wallScript = wall.GetComponent<WallScript>();
        GraphicWallScript graphicWallScript = wall.GetComponent<GraphicWallScript>();
        wallScript.gameEnv = gameEnv;
        wallScript.identifier = template.identifier;
        wall.name = template.identifier + "_" + System.Guid.NewGuid();
        gameEnv.wallDict[wall.name] = wall;

        wallScript.graphicObjectPrefab = template.graphicObjectPrefab;

        wallScript.type = template.type;
        wallScript.materialTypes = template.materialTypes;
        wallScript.materialTypesDistribution = template.materialTypesDistribution;

        Value transparency_calculation = parseTemplateVariable(template.transparency, ref gameEnv);
        if (transparency_calculation.valueType == "double") {
            wallScript.transparency = (float)transparency_calculation.vDouble;
        } else {
            // Throw Error
        } 

        foreach (Effect effect in template.effects) {
            if (effect.stacks) {
                effect.givenName = effect.name + "_" + System.Guid.NewGuid();
            } else {
                effect.givenName = effect.name;
            }
            wallScript.effects[effect.givenName] = effect;
        }

        foreach (KeyValuePair<string, TemplateVariable> var in template.initVariableList) {
            wallScript.variables[var.Key] = parseTemplateVariable(var.Value, ref gameEnv);
        }

        GameObject graphicWall = GameObject.Instantiate(wallScript.graphicObjectPrefab, wall.transform, false);
        // graphicWall.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        graphicWall.SetActive(false);
        graphicWallScript.wallScript = wallScript;
        graphicWallScript.graphicObject_transform = graphicWall.transform;
        graphicWallScript.graphicObject_collider = graphicWall.transform.GetComponent<Collider>();
        wallScript.graphicObject = graphicWall;
        wallScript.graphicWallScript = graphicWallScript;

        return wall;
    }

    static public void placeGraphicWall(ref GameObject wall, Index index_1, Index index_2, float rotation) {
        WallScript wallScript = wall.GetComponent<WallScript>();
        wallScript.onMap = true;
        float cubeSize = wallScript.gameEnv.cubeSize;

        if (index_1.x == index_2.x) {
            if (index_1.z > index_2.z) {
                wallScript.gameEnv.mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_z = wall;
                wallScript.gameEnv.mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_zz = wall;
                wallScript.index_z = index_1;
                wallScript.index_zz = index_2;
            } else {
                wallScript.gameEnv.mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_zz = wall;
                wallScript.gameEnv.mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_z = wall;
                wallScript.index_zz = index_1;
                wallScript.index_z = index_2;
            }
        } else {
            if (index_1.x > index_2.x) {
                wallScript.gameEnv.mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_x = wall;
                wallScript.gameEnv.mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_xx = wall;
                wallScript.index_x = index_1;
                wallScript.index_xx = index_2;
            } else {
                wallScript.gameEnv.mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_xx = wall;
                wallScript.gameEnv.mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_x = wall;
                wallScript.index_xx = index_1;
                wallScript.index_x = index_2;
            }
        }

        Vector3 placePos = new Vector3((index_1.x + index_2.x) * cubeSize / 2,(index_1.y + index_2.y) * cubeSize / 2, (index_1.z + index_2.z) * cubeSize / 2);
        wallScript.graphicObject.SetActive(true);
        wall.transform.position = placePos;
        wallScript.graphicObject.transform.eulerAngles = new Vector3(0, rotation, 0);
        // wallScript.graphicObject.GetComponent<GraphicWallScript>().goingTo = placePos;
    }

    static public void placeGraphicWall(ref GameObject wall, Vector3 pos, float rotation) {
        WallScript wallScript = wall.GetComponent<WallScript>();
        wallScript.onMap = true;
        float cubeSize = wallScript.gameEnv.cubeSize;

        Index index_1 = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));
        Index index_2 = new Index(Mathf.CeilToInt(pos.x / cubeSize), Mathf.CeilToInt(pos.y / cubeSize), Mathf.CeilToInt(pos.z / cubeSize));

        if (index_1.x == index_2.x) {
            if (index_1.z > index_2.z) {
                wallScript.gameEnv.mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_z = wall;
                wallScript.index_z = index_1;
                wallScript.gameEnv.mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_zz = wall;
                wallScript.index_zz = index_2;
            } else {
                wallScript.gameEnv.mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_zz = wall;
                wallScript.index_zz = index_1;
                wallScript.gameEnv.mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_z = wall;
                wallScript.index_z = index_2;
            }
        } else {
            if (index_1.x > index_2.x) {
                wallScript.gameEnv.mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_x = wall;
                wallScript.index_x = index_1;
                wallScript.gameEnv.mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_xx = wall;
                wallScript.index_xx = index_2;
            } else {
                wallScript.gameEnv.mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_xx = wall;
                wallScript.index_xx = index_1;
                wallScript.gameEnv.mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_x = wall;
                wallScript.index_x = index_2;
            }
        }

        wallScript.graphicObject.SetActive(true);
        wall.transform.position = pos;
        wallScript.graphicObject.transform.eulerAngles = new Vector3(0, rotation, 0);
        // shapeScript.graphicObject.GetComponent<GraphicShapeScript>().goingTo = placePos;
    }

    static public void deleteGraphicWall(ref GameObject wall) {
        WallScript wallScript = wall.GetComponent<WallScript>();
        Index index_x = wallScript.index_x;
        Index index_xx = wallScript.index_xx;
        Index index_z = wallScript.index_z;
        Index index_zz = wallScript.index_zz;
        if (index_x != null) {
            wallScript.gameEnv.mapScript.gameBoard[index_x.x, index_x.y, index_x.z].wall_x = null;
        }
        if (index_z != null) {
            wallScript.gameEnv.mapScript.gameBoard[index_z.x, index_z.y, index_z.z].wall_z = null;
        }
        if (index_xx != null) {
            wallScript.gameEnv.mapScript.gameBoard[index_xx.x, index_xx.y, index_xx.z].wall_xx = null;
        }
        if (index_zz != null) {
            wallScript.gameEnv.mapScript.gameBoard[index_zz.x, index_zz.y, index_zz.z].wall_x = null;
        }
        wallScript.graphicObject.SetActive(false);
        wallScript.onMap = false;
    }

    static public void deleteWall(ref GameObject wall) {
        WallScript wallScript = wall.GetComponent<WallScript>();
        if (wallScript.onMap) {
            deleteGraphicWall(ref wall);
        }
        GameObject.Destroy(wall);
    }

    static public void deleteWallAtPos(Vector3 pos, ref MapScript mapScript) {
        float cubeSize = mapScript.cubeSize;

        Index index_1 = new Index(Mathf.FloorToInt(pos.x / cubeSize), Mathf.FloorToInt(pos.y / cubeSize), Mathf.FloorToInt(pos.z / cubeSize));
        Index index_2 = new Index(Mathf.CeilToInt(pos.x / cubeSize), Mathf.CeilToInt(pos.y / cubeSize), Mathf.CeilToInt(pos.z / cubeSize));

        GameObject wall;

        if (index_1.x == index_2.x) {
            if (index_1.z > index_2.z) {
                wall = mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_z;
                mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_z = null;
                mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_zz = null;
            } else {
                wall = mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_zz;
                mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_zz = null;
                mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_z = null;
            }
        } else {
            if (index_1.x > index_2.x) {
                wall = mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_x;
                mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_x = null;
                mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_xx = null;
            } else {
                wall = mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_xx;
                mapScript.gameBoard[index_1.x, index_1.y, index_1.z].wall_xx = null;
                mapScript.gameBoard[index_2.x, index_2.y, index_2.z].wall_x = null;
            }
        }

        GameObject.Destroy(wall);
    }

    // ==========================================================================================================================
    // ==========================================================================================================================
    // ==========================================================================================================================
    // ==========================================================================================================================
    // ========================================================================================================================== Testing Functions
    static public void createAndPlaceToken(GameObject tokenPrefab, TokenTemplate template, ref GameEnv gameEnv, Index pos) {
        GameObject token = quickCreateToken(tokenPrefab, template, ref gameEnv);
        placeGraphicToken(ref token, pos);
    }

    static public void createAndPlaceShape(GameObject shapePrefab, ShapeTemplate template, ref GameEnv gameEnv, Vector3 pos) {
        GameObject cube = quickCreateShape(shapePrefab, template, ref gameEnv);
        placeGraphicShape(ref cube, pos);
    }

    static public void createAndPlaceWall(GameObject wallPrefab, WallTemplate template, ref GameEnv gameEnv, Vector3 pos, float rotation) {
        GameObject cube = quickCreateWall(wallPrefab, template, ref gameEnv);
        placeGraphicWall(ref cube, pos, rotation);
    }

    // ==========================================================================================================================
    // ==========================================================================================================================
    // ==========================================================================================================================
    // ==========================================================================================================================
    // ========================================================================================================================== General Utility
    static public Index GetIndex(GameObject gameObject) {
        TokenScript tokenScript = gameObject.GetComponent<TokenScript>();
        if (tokenScript != null) {
            return tokenScript.index;
        } else {
            ShapeScript cubeScript = gameObject.GetComponent<ShapeScript>();
            return cubeScript.index;
        }
    }

}