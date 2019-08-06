using System;
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

    void Start()
    {
        typeDict = new Dictionary<string, Type>();
        typeDict.Add("testCube", new Type("cube"));
        typeDict.Add("testToken", new Type("token"));

        gameEnv = new GameEnv();

        gameEnv.cubesObject = GameObject.Find("Cubes");
        gameEnv.tokensObject = GameObject.Find("Tokens");
        gameEnv.mapScript = GameObject.Find("Map").GetComponent<MapScript>();

        AddActions();
        AddTokenTemplates();
        addCubeTemplates();
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
        Action nullAction = new Action(
            "Null Action",
            new Type("action"),
            new List<string> {"true"},
            new List<Effect>(),
            0,
            new List<Raction>(),
            new List<Taction>(),
            new List<Action>(),
            new List<Raction>(),
            new List<Taction>()
        );

        gameEnv.actionDict.Add(nullAction.name, nullAction);
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
            new List<Action> {gameEnv.actionDict["Null Action"]},
            new List<Raction>(),
            new List<Taction>(),
            new Dictionary<string, TemplateVariable> {
                { "Hit Points", new TemplateVariable(4, 10) }, 
                { "Strength", new TemplateVariable(6, 10) },
                { "Dexterity", new TemplateVariable(11, 16) },
                { "Constituion", new TemplateVariable(8, 11) },
                { "Intelligence", new TemplateVariable(8, 12) },
                { "Wisdom", new TemplateVariable(3, 11) },
                { "Charisma", new TemplateVariable(4, 12) },
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
            new List<Action> {gameEnv.actionDict["Null Action"]},
            new List<Raction>(),
            new List<Taction>(),
            new Dictionary<string, TemplateVariable> {
                { "Hit Points", new TemplateVariable(8, 16) }, 
                { "Strength", new TemplateVariable(10, 14) },
                { "Dexterity", new TemplateVariable(10, 14) },
                { "Constituion", new TemplateVariable(10, 14) },
                { "Intelligence", new TemplateVariable(10, 14) },
                { "Wisdom", new TemplateVariable(10, 14) },
                { "Charisma", new TemplateVariable(10, 14) },
            }
        );

        gameEnv.tokenTemplates.Add(goblin.identifier, goblin);
        gameEnv.tokenTemplates.Add(adventurer.identifier, adventurer);
    }

    public void addCubeTemplates() {
        CubeTemplate stone = new CubeTemplate(
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

        gameEnv.cubeTemplates.Add(stone.identifier, stone);
    }
}
