using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnvScript : MonoBehaviour
{
    public GameEnv gameEnv;
    int lastGameTime;

    Dictionary<string, Type> typeDict;

    public List<Event> gameTimeEvents;
    public List<Event> realTimeEvents;

    public GameObject goblinPrefab;
    public GameObject adventurerPrefab;
    public GameObject stonePrefab;
    public GameObject wallPrefab;

    void Start()
    {
        typeDict = new Dictionary<string, Type>();
        typeDict.Add("testCube", new Type("cube"));
        typeDict.Add("testToken", new Type("token"));

        gameEnv = new GameEnv();

        gameEnv.shapesObject = GameObject.Find("Shapes");
        gameEnv.tokensObject = GameObject.Find("Tokens");
        gameEnv.mapScript = GameObject.Find("Map").GetComponent<MapScript>();

        populateStore();
        AddActions();
        AddTokenTemplates();
        addCubeTemplates();
        addWallTemplates();
    }

    void Update()
    {
        // foreach (Event realTimeEvent in realTimeEvents) {
        //     // TODO
        //     resolveEvent(realTimeEvent, ref gameEnv);
        //     removeEndedEvent(realTimeEvent, ref realTimeEvents);
        // }

        // if (gameEnv.inGameTime != lastGameTime) {
        //     lastGameTime = gameEnv.inGameTime;
        //     foreach (Event gameTimeEvent in gameTimeEvents) {
        //         ResolveEventArgs(gameTimeEvent, ref gameEnv);
        //         removeEndedEvent(gameTimeEvent, ref gameTimeEvents);
        //     }
        // }
    }

    
    
    public void AddActions() {
        Effect nullActionEffect = Utils.createEffect(
            new Dictionary<string, string>() { 
                { "name", "Null-Action-Effect" } 
            },
            new Dictionary<string, bool>() { 
                { "relational", false },
                { "targeted", false },
                { "instant", true },
            },
            null,
            new Dictionary<string, List<string>>() { 
                { "scripts", new List<string> { "Null_Script();" } } 
            },
            null
        );

        Action nullAction = new Action(
            "Null-Action",
            4,
            null,
            null,
            null,
            new List<Effect> { nullActionEffect },
            -1,
            null,
            null,
            null,
            null
        );

        Effect BasicMoveEffect = new Effect(
            "Basic-Move-Effect",
            null,
            new List<string> { "Basic_Move_Script($self, $target);" },
            "Get_Shape_Top($target);",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        Action BasicMove = new Action(
            "Basic-Move",
            1,
            null,
            new List<string> { "Can_Move($self);" },
            new List<string> { "Basic_Move_Call_Condition($self, $target);"},
            null,
            new List<Effect> { BasicMoveEffect },
            -1,
            null,
            null,
            null,
            null
        );

        Effect ResolveBasicAttackEffect = new Effect(
            "Resolve-Basic-Attack-Effect",
            null,
            new List<string> { "Basic_Attack_Script($self, $target);" },
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );
        
        Action ResolveBasicAttack = new Action(
            "Resolve-Basic-Attack",
            0,
            null,
            null,
            null,
            new List<string> { "Basic_Attack_Hit_Condition($self, $target)" },
            new List<Effect> { ResolveBasicAttackEffect },
            -1,
            null,
            null,
            null,
            null,
            null,
            null
        );

        Effect BasicAttackEffect = new Effect(
            "Basic-Attack-Effect",
            null,
            new List<string> { "Deduct_Standard_Action($self);" },
            null,
            null,
            null,
            null,
            null,
            null
        );

        Action BasicAttack = new Action(
            "Basic-Attack",
            2,
            null,
            new List<string> { "Has_Standard_Action($self);" },
            null,
            null,
            new List<Effect> { BasicAttackEffect },
            -1,
            null,
            null,
            new List<Action> { ResolveBasicAttack },
            null,
            null,
            null
        );

        gameEnv.actionDict.Add(nullAction.name, nullAction);
        gameEnv.actionDict.Add(BasicAttack.name, BasicAttack);
        gameEnv.actionDict.Add(BasicMove.name, BasicMove);
    }

    public void AddTokenTemplates() {
        TokenTemplate goblin = new TokenTemplate(
            goblinPrefab,
            "goblin",
            new TemplateVariable(new List<string> {"grok", "boblin", "tiptip"}),
            new Type("goblin"),
            new List<Type>(),
            new List<float>(),
            1,
            1,
            1,
            new List<Effect>(),
            new List<Action> {
                gameEnv.actionDict["Null-Action"],
                gameEnv.actionDict["Basic-Attack"],
                gameEnv.actionDict["Basic-Move"],
            },
            new Dictionary<string, TemplateVariable> {
                { "hp", new TemplateVariable(4, 10) }, 
                { "str", new TemplateVariable(6, 10) },
                { "dex", new TemplateVariable(11, 16) },
                { "con", new TemplateVariable(8, 11) },
                { "int", new TemplateVariable(8, 12) },
                { "wis", new TemplateVariable(3, 11) },
                { "cha", new TemplateVariable(4, 12) },
                { "bab", new TemplateVariable(2) },
                { "weapon_damage", new TemplateVariable(8) },
                { "AC", new TemplateVariable(15) },
                { "standard_action_score", new TemplateVariable(5) },
                { "standard_action_points", new TemplateVariable(5) },
                { "move_action_score", new TemplateVariable(6) },
                { "move_action_points", new TemplateVariable(6) },
                { "move_score", new TemplateVariable(6) },
                { "move_points", new TemplateVariable(6) },
                { "move_cheap_diagnol", new TemplateVariable(true) },
            }
        );

        TokenTemplate adventurer = new TokenTemplate(
            adventurerPrefab,
            "adventurer",
            new TemplateVariable(new List<string> {"Thom", "Eli", "Ara"}),
            new Type("adventurer"),
            new List<Type>(),
            new List<float>(),
            1,
            1,
            2,
            new List<Effect>(),
            new List<Action> {
                gameEnv.actionDict["Null-Action"],
                gameEnv.actionDict["Basic-Attack"],
                gameEnv.actionDict["Basic-Move"],
            },
            new Dictionary<string, TemplateVariable> {
                { "hp", new TemplateVariable(8, 16) }, 
                { "str", new TemplateVariable(10, 14) },
                { "dex", new TemplateVariable(10, 14) },
                { "con", new TemplateVariable(10, 14) },
                { "int", new TemplateVariable(10, 14) },
                { "wis", new TemplateVariable(10, 14) },
                { "cha", new TemplateVariable(10, 14) },
                { "bab", new TemplateVariable(4) },
                { "weapon_damage", new TemplateVariable(10) },
                { "AC", new TemplateVariable(18) },
                { "standard_action_score", new TemplateVariable(5) },
                { "standard_action_points", new TemplateVariable(5) },
                { "move_action_score", new TemplateVariable(6) },
                { "move_action_points", new TemplateVariable(6) },
                { "move_score", new TemplateVariable(6) },
                { "move_points", new TemplateVariable(6) },
                { "move_cheap_diagnol", new TemplateVariable(true) },
            }
        );

        gameEnv.tokenTemplates.Add(goblin.identifier, goblin);
        gameEnv.tokenTemplates.Add(adventurer.identifier, adventurer);
    }

    public void addCubeTemplates() {
        ShapeTemplate stone = new ShapeTemplate(
            stonePrefab,
            "stone",
            new Type("stone"),
            new List<Type>(),
            new List<float>(),
            new TemplateVariable(0.0), 
            new List<Effect>(),
            new Dictionary<string, TemplateVariable> {
                { "Passable", new TemplateVariable(true) }, 
                { "Difficult Terrain", new TemplateVariable(false) }, 
                { "Hit Points", new TemplateVariable(80, 100) },
            }
        );

        gameEnv.shapeTemplates.Add(stone.identifier, stone);
    }

    public void addWallTemplates() {
        WallTemplate stone = new WallTemplate(
            wallPrefab,
            "stone",
            new Type("stone"),
            new List<Type>(),
            new List<float>(),
            new TemplateVariable(0.0), 
            new List<Effect>(),
            new Dictionary<string, TemplateVariable> {
                { "Passable", new TemplateVariable(true) }, 
                { "Difficult Terrain", new TemplateVariable(false) }, 
                { "Hit Points", new TemplateVariable(80, 100) },
            }
        );

        gameEnv.wallTemplates.Add(stone.identifier, stone);
    }

    void addScriptToStore(string script, ref GameEnv gameEnv) {
        Result result = DaedScript.evaluateToResult(script, ref gameEnv);
        if (result.value.errorMessage != null) {
            Debug.Log(result.value.errorMessage);
        }
        if (result.store != null && result.env != null) {
            gameEnv.store = result.store;
            gameEnv.env = result.env;
            Utils.printEnvStore(gameEnv.env, gameEnv.store);
        }
    }

    public void populateStore() {
        string path = "Assets/GameLogic/pathfinder_test.txt";
        string pathfinderScript = Utils.readTextFile(path);
        addScriptToStore(pathfinderScript, ref gameEnv);
    }
}
