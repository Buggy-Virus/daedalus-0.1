using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditEffectScript : MonoBehaviour
{
    public float moveDistance = 30;
    public GameEnv gameEnv;

    public GameObject panel;
    public GameObject scriptInputPrefab;
    public GameObject actionInputPrefab;
    public GameObject effectInputPrefab;

    public GameObject nameText;
    public GameObject nameInput;
    public GameObject relationalText;
    public GameObject relationalToggle;
    public GameObject targetedText;
    public GameObject targetedToggle;
    public GameObject stacksText;
    public GameObject stacksToggle;
    public GameObject instantText;
    public GameObject instantToggle;
    public GameObject procCondText;
    public GameObject procCondAdd;
    public GameObject procCondRemove;
    public GameObject endCondText;
    public GameObject endCondAdd;
    public GameObject endCondRemove;
    public GameObject frequencyText;
    public GameObject frequencyInput;
    public GameObject scriptsText;
    public GameObject scriptsAdd;
    public GameObject scriptsRemove;
    public GameObject selfDisplaceText;
    public GameObject selfDisplaceInput;
    public GameObject selfDisplacePerpText;
    public GameObject selfDisplacePerpInput;
    public GameObject selfDisplaceParaText;
    public GameObject selfDisplaceParaInput;
    public GameObject selfDisplaceAltText;
    public GameObject selfDisplaceAltInput;
    public GameObject targetDisplaceText;
    public GameObject targetDisplaceInput;
    public GameObject targetDisplacePerpText;
    public GameObject targetDisplacePerpInput;
    public GameObject targetDisplaceParaText;
    public GameObject targetDisplaceParaInput;
    public GameObject targetDisplaceAltText;
    public GameObject targetDisplaceAltInput;
    public GameObject followupActionsText;
    public GameObject followupActionsAdd;
    public GameObject followupActionsRemove;
    public GameObject targetFollowupActionsText;
    public GameObject targetFollowupActionsAdd;
    public GameObject targetFollowupActionsRemove;
    public GameObject targetedFollowupActionsText;
    public GameObject targetedFollowupActionsAdd;
    public GameObject targetedFollowupActionsRemove;
    public GameObject conditionsText;
    public GameObject conditionsAdd;
    public GameObject conditionsRemove;
    public GameObject cond_scriptsText;
    public GameObject cond_scriptsAdd;
    public GameObject cond_scriptsRemove;
    public GameObject cond_selfDisplaceText;
    public GameObject cond_selfDisplaceInput;
    public GameObject cond_selfDisplacePerpText;
    public GameObject cond_selfDisplacePerpInput;
    public GameObject cond_selfDisplaceParaText;
    public GameObject cond_selfDisplaceParaInput;
    public GameObject cond_selfDisplaceAltText;
    public GameObject cond_selfDisplaceAltInput;
    public GameObject cond_targetDisplaceText;
    public GameObject cond_targetDisplaceInput;
    public GameObject cond_targetDisplacePerpText;
    public GameObject cond_targetDisplacePerpInput;
    public GameObject cond_targetDisplaceParaText;
    public GameObject cond_targetDisplaceParaInput;
    public GameObject cond_targetDisplaceAltText;
    public GameObject cond_targetDisplaceAltInput;
    public GameObject cond_followupActionsText;
    public GameObject cond_followupActionsAdd;
    public GameObject cond_followupActionsRemove;
    public GameObject cond_targetFollowupActionsText;
    public GameObject cond_targetFollowupActionsAdd;
    public GameObject cond_targetFollowupActionsRemove;
    public GameObject cond_targetedFollowupActionsText;
    public GameObject cond_targetedFollowupActionsAdd;
    public GameObject cond_targetedFollowupActionsRemove;

    List<GameObject> procCondInputs;
    List<GameObject> endCondInputs;
    List<GameObject> scriptsInputs;
    List<GameObject> followupActionsInputs;
    List<GameObject> targetFollowupActionsInputs;
    List<GameObject> targetedFollowupActionsInputs;
    List<GameObject> conditionsInputs;
    List<GameObject> cond_scriptsInputs;
    List<GameObject> cond_followupActionsInputs;
    List<GameObject> cond_targetFollowupActionsInputs;
    List<GameObject> cond_targetedFollowupActionsInputs;
    List<GameObject> cond_conditionsInputs;
    

    public GameObject loadEffect_input;
    public GameObject loadEffect_button;
    public GameObject editEffect_button;
    public GameObject clearEffect_button;

    List<GameObject> orderedElements;

    void moveElements(int elementsToMoveIndex, float amountToMove) {
        foreach (GameObject elem in orderedElements.GetRange(elementsToMoveIndex, orderedElements.Count - elementsToMoveIndex)) {
            elem.transform.position += new Vector3(0, amountToMove, 0);
        }
    }

    System.Action AddScriptInput(GameObject text, GameObject addButton, GameObject removeButton, List<GameObject> inputList) {
        return delegate() {
            int elementsToMoveIndex = orderedElements.IndexOf(text) + 1;
            addButton.transform.position += new Vector3(0, -moveDistance, 0);
            removeButton.transform.position += new Vector3(0, -moveDistance, 0);
            moveElements(elementsToMoveIndex, -moveDistance);

            GameObject input = GameObject.Instantiate(scriptInputPrefab, panel.transform);
            float x_position = nameInput.transform.position.x;
            float y_position = text.transform.position.y;
            input.transform.position = new Vector3(x_position, y_position, 0);

            orderedElements.Insert(elementsToMoveIndex, input);
            inputList.Add(input);
        };
    }

    System.Action AddActionInput(GameObject text, GameObject addButton, GameObject removeButton, List<GameObject> inputList) {
        return delegate() {
            int elementsToMoveIndex = orderedElements.IndexOf(text) + 1;
            addButton.transform.position += new Vector3(0, -moveDistance, 0);
            removeButton.transform.position += new Vector3(0, -moveDistance, 0);
            moveElements(elementsToMoveIndex, -moveDistance);

            GameObject input = GameObject.Instantiate(actionInputPrefab, panel.transform);
            float x_position = nameInput.transform.position.x;
            float y_position = text.transform.position.y;
            input.transform.position = new Vector3(x_position, y_position, 0);

            orderedElements.Insert(elementsToMoveIndex, input);
            inputList.Add(input);
        };
    }

    System.Action AddEffectInput(GameObject text, GameObject addButton, GameObject removeButton, List<GameObject> inputList) {
        return delegate() {
            int elementsToMoveIndex = orderedElements.IndexOf(text) + 1;
            addButton.transform.position += new Vector3(0, -moveDistance, 0);
            removeButton.transform.position += new Vector3(0, -moveDistance, 0);
            moveElements(elementsToMoveIndex, -moveDistance);

            GameObject input = GameObject.Instantiate(effectInputPrefab, panel.transform);
            float x_position = nameInput.transform.position.x;
            float y_position = text.transform.position.y;
            input.transform.position = new Vector3(x_position, y_position, 0);

            orderedElements.Insert(elementsToMoveIndex, input);
            inputList.Add(input);
        };
    }

    System.Action RemoveInput(List<GameObject> inputList, GameObject addButton, GameObject removeButton) {
        return delegate() {
            // panel.GetComponent<RectTransform>().sizeDelta = new Vector2 (0, -30);  
            if (inputList.Count != 0) {
                GameObject lastInput = inputList[inputList.Count - 1];
                int elementsToMoveIndex = orderedElements.IndexOf(lastInput);
                orderedElements.Remove(lastInput);
                inputList.Remove(lastInput);
                GameObject.Destroy(lastInput);

                addButton.transform.position += new Vector3(0, moveDistance, 0);
                removeButton.transform.position += new Vector3(0, moveDistance, 0);
                moveElements(elementsToMoveIndex, moveDistance);
            }  
        };
    }

    void AddScripts(List<GameObject> inputs, ref List<string> scripts) {
        if (inputs.Count > 0) {
            scripts = new List<string>();
            foreach (GameObject input in inputs) {
                string inputText = input.GetComponent<InputField>().text;
                if (inputText != "") {
                    scripts.Add(inputText);
                }
            }
        }
    }

    bool AddEffects(List<GameObject> inputs, ref List<Effect> effects, ref string error) {
        if (inputs.Count > 0) {
            effects = new List<Effect>();
            foreach (GameObject input in inputs) {
                string inputText = input.GetComponent<InputField>().text;
                if (inputText != "") {
                    if (gameEnv.effectDict.ContainsKey(inputText)) {
                        effects.Add(gameEnv.effectDict[inputText]);
                    } else {
                        error += "Cannot add effect, no effect of name: " + inputText;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    bool AddActions(List<GameObject> inputs, ref List<Action> actions, ref string error, bool relational, bool targeted, bool either) {
        if (inputs.Count > 0) {
            actions = new List<Action>();
            foreach (GameObject input in inputs) {
                string inputText = input.GetComponent<InputField>().text;
                if (inputText != "") {
                    if (!gameEnv.actionDict.ContainsKey(inputText)) {
                        error += "Cannot add action, no action of name: " + inputText;
                        return true;
                    } else if (relational && !gameEnv.actionDict[inputText].relational) {
                        error += "Cannot add action, following action is not relatioanl: " + inputText;
                        return true;
                    } else if (targeted && !gameEnv.actionDict[inputText].targeted) {
                        error += "Cannot add action, following action is not targeted: " + inputText;
                        return true;
                    } else if (either && !gameEnv.actionDict[inputText].relational && !gameEnv.actionDict[inputText].targeted) {
                        error += "Cannot add action, following action is not relational or targeted: " + inputText;
                        return true;
                    } else {
                        actions.Add(gameEnv.actionDict[inputText]);
                    }
                }
            }
        }

        return false;
    }

    int EditEffectHelper(ref string error) {
        Effect editEffect = new Effect();
        editEffect.name = nameInput.GetComponent<InputField>().text;
        if (editEffect.name == "") {
            error += "Action name cannot be blank";
            return 1;
        } 

        editEffect.relational = relationalToggle.GetComponent<Toggle>().isOn;
        editEffect.targeted = targetedToggle.GetComponent<Toggle>().isOn;

        editEffect.stacks = stacksToggle.GetComponent<Toggle>().isOn;

        editEffect.instant = instantToggle.GetComponent<Toggle>().isOn;
        AddScripts(procCondInputs, ref editEffect.procConditions);
        AddScripts(endCondInputs, ref editEffect.endConditions);
        if (Int32.TryParse(frequencyInput.GetComponent<InputField>().text, out editEffect.frequency)) {
            error += "Min Range must be an int";
            return 1;
        }

        AddScripts(scriptsInputs, ref editEffect.scripts);

        editEffect.self_displace = selfDisplaceInput.GetComponent<InputField>().text;
        editEffect.self_displace_perp = selfDisplacePerpInput.GetComponent<InputField>().text;
        editEffect.self_displace_para = selfDisplaceParaInput.GetComponent<InputField>().text;
        editEffect.self_displace_perp = selfDisplaceAltInput.GetComponent<InputField>().text;

        editEffect.target_displace = targetDisplaceInput.GetComponent<InputField>().text;
        editEffect.target_displace_perp = targetDisplacePerpInput.GetComponent<InputField>().text;
        editEffect.target_displace_para = targetDisplaceParaInput.GetComponent<InputField>().text;
        editEffect.target_displace_perp = targetDisplaceAltInput.GetComponent<InputField>().text;

        if (AddActions(followupActionsInputs, ref editEffect.followup_actions, ref error, false, false, false)) {
            return 1;
        }
        if (AddActions(targetFollowupActionsInputs, ref editEffect.target_followup_actions, ref error, false, false, false)) {
            return 1;
        }
        if (AddActions(targetedFollowupActionsInputs, ref editEffect.targeted_followup_actions, ref error, false, false, true)) {
            return 1;
        }

        AddScripts(conditionsInputs, ref editEffect.conditions);

        AddScripts(scriptsInputs, ref editEffect.scripts);

        editEffect.conditional_self_displace = cond_targetDisplaceInput.GetComponent<InputField>().text;
        editEffect.conditional_self_displace_perp = cond_selfDisplacePerpInput.GetComponent<InputField>().text;
        editEffect.conditional_self_displace_para = cond_selfDisplaceParaInput.GetComponent<InputField>().text;
        editEffect.conditional_self_displace_perp = cond_selfDisplaceAltInput.GetComponent<InputField>().text;

        editEffect.conditional_target_displace = cond_selfDisplaceInput.GetComponent<InputField>().text;
        editEffect.conditional_target_displace_perp = cond_targetDisplacePerpInput.GetComponent<InputField>().text;
        editEffect.conditional_target_displace_para = cond_targetDisplaceParaInput.GetComponent<InputField>().text;
        editEffect.conditional_target_displace_perp = cond_targetDisplaceAltInput.GetComponent<InputField>().text;

        if (AddActions(cond_followupActionsInputs, ref editEffect.conditional_followup_actions, ref error, false, false, false)) {
            return 1;
        }
        if (AddActions(cond_targetFollowupActionsInputs, ref editEffect.conditional_target_followup_actions, ref error, false, false, false)) {
            return 1;
        }
        if (AddActions(cond_targetedFollowupActionsInputs, ref editEffect.conditional_targeted_followup_actions, ref error, false, false, true)) {
            return 1;
        }

        return 0;
    }

    public void EditEffect() {
        string error = "Edit Effect Error: ";
        int result = EditEffectHelper(ref error);
        if (result == 1) {
            Debug.Log(error);
            gameEnv.console.ConsoleLog(error);
        }
    }

    public void LoadScripts(List<string> scripts, ref GameObject uiText, ref GameObject uiAdd, ref GameObject uiRemove, ref List<GameObject> uiInputs) {
        if (scripts != null) {
            foreach(String script in scripts) {
                AddScriptInput(uiText, uiAdd, uiRemove, uiInputs)();
                uiInputs[uiInputs.Count - 1].GetComponent<InputField>().text = script;
            }
        }
    }

    public void LoadActions(List<Action> actions, ref GameObject uiText, ref GameObject uiAdd, ref GameObject uiRemove, ref List<GameObject> uiInputs) {
        if (actions != null) {
            foreach(Action action in actions) {
                AddActionInput(uiText, uiAdd, uiRemove, uiInputs)();
                uiInputs[uiInputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }
    }

    public void LoadEffects(List<Effect> effects, ref GameObject uiText, ref GameObject uiAdd, ref GameObject uiRemove, ref List<GameObject> uiInputs) {
        if (effects != null) {
            foreach(Effect effect in effects) {
                AddEffectInput(uiText, uiAdd, uiRemove, uiInputs)();
                uiInputs[uiInputs.Count - 1].GetComponent<InputField>().text = effect.name;
            }
        }
    }

    public void loadEditEffect(Effect load) {
        nameInput.GetComponent<InputField>().text = load.name;

        relationalToggle.GetComponent<Toggle>().isOn = load.relational;
        targetedToggle.GetComponent<Toggle>().isOn = load.targeted;

        stacksToggle.GetComponent<Toggle>().isOn = load.stacks;

        instantToggle.GetComponent<Toggle>().isOn = load.instant;
        LoadScripts(load.procConditions, ref procCondText, ref procCondAdd, ref procCondRemove, ref procCondInputs);
        LoadScripts(load.endConditions, ref endCondText, ref endCondAdd, ref endCondRemove, ref endCondInputs);
        frequencyInput.GetComponent<InputField>().text = load.frequency.ToString();

        LoadScripts(load.scripts, ref scriptsText, ref scriptsAdd, ref scriptsRemove, ref scriptsInputs);

        selfDisplaceInput.GetComponent<InputField>().text = load.self_displace.ToString();
        selfDisplacePerpInput.GetComponent<InputField>().text = load.self_displace_perp.ToString();
        selfDisplaceParaInput.GetComponent<InputField>().text = load.self_displace_para.ToString();
        selfDisplaceAltInput.GetComponent<InputField>().text = load.self_displace_alt.ToString();

        targetDisplaceInput.GetComponent<InputField>().text = load.target_displace.ToString();
        targetDisplacePerpInput.GetComponent<InputField>().text = load.target_displace_perp.ToString();
        targetDisplaceParaInput.GetComponent<InputField>().text = load.target_displace_para.ToString();
        targetDisplaceAltInput.GetComponent<InputField>().text = load.target_displace_alt.ToString();

        LoadActions(load.followup_actions, ref followupActionsText, ref followupActionsAdd, ref followupActionsRemove, ref followupActionsInputs);
        LoadActions(load.target_followup_actions, ref targetFollowupActionsText, ref targetFollowupActionsAdd, ref targetFollowupActionsRemove, ref targetFollowupActionsInputs);
        LoadActions(load.targeted_followup_actions, ref targetedFollowupActionsText, ref targetedFollowupActionsAdd, ref targetedFollowupActionsRemove, ref followupActionsInputs);

        LoadScripts(load.conditions, ref conditionsText, ref conditionsAdd, ref conditionsRemove, ref conditionsInputs);

        LoadScripts(load.conditional_scripts, ref cond_scriptsText, ref cond_scriptsAdd, ref cond_scriptsRemove, ref cond_scriptsInputs);

        cond_selfDisplaceInput.GetComponent<InputField>().text = load.conditional_self_displace.ToString();
        cond_selfDisplacePerpInput.GetComponent<InputField>().text = load.conditional_self_displace_perp.ToString();
        cond_selfDisplaceParaInput.GetComponent<InputField>().text = load.conditional_self_displace_para.ToString();
        cond_selfDisplaceAltInput.GetComponent<InputField>().text = load.conditional_self_displace_alt.ToString();

        cond_targetDisplaceInput.GetComponent<InputField>().text = load.conditional_target_displace.ToString();
        cond_targetDisplacePerpInput.GetComponent<InputField>().text = load.conditional_target_displace_perp.ToString();
        cond_targetDisplaceParaInput.GetComponent<InputField>().text = load.conditional_target_displace_para.ToString();
        cond_targetDisplaceAltInput.GetComponent<InputField>().text = load.conditional_target_displace_alt.ToString();

        LoadActions(load.conditional_followup_actions, ref cond_followupActionsText, ref cond_followupActionsAdd, ref cond_followupActionsRemove, ref cond_followupActionsInputs);
        LoadActions(load.conditional_target_followup_actions, ref cond_targetFollowupActionsText, ref cond_targetFollowupActionsAdd, ref cond_targetFollowupActionsRemove, ref cond_targetFollowupActionsInputs);
        LoadActions(load.conditional_targeted_followup_actions, ref cond_targetedFollowupActionsText, ref cond_targetedFollowupActionsAdd, ref cond_targetedFollowupActionsRemove, ref followupActionsInputs);
    }

    public void loadInputEffect() {
        string effectToLoad = loadEffect_input.GetComponent<InputField>().text;
        if (gameEnv.effectDict.ContainsKey(effectToLoad)) {
            clearEditEffect();
            loadEditEffect(gameEnv.effectDict[effectToLoad]);
        } else {
            Debug.Log("No effect named \"" + effectToLoad + "\"");
            gameEnv.console.ConsoleLog("No effect named \"" + effectToLoad + "\"");
        }
    }

    void ClearInputs(ref GameObject addButton, ref GameObject removeButton, ref List<GameObject> inputs) {
        for (int i = 0; i < inputs.Count; i++) {
            RemoveInput(inputs, removeButton, addButton);
        }
    }

    public void clearEditEffect() {
        nameInput.GetComponent<InputField>().text = "";

        relationalToggle.GetComponent<Toggle>().isOn = false;
        targetedToggle.GetComponent<Toggle>().isOn = false;
        stacksToggle.GetComponent<Toggle>().isOn = false;

        instantToggle.GetComponent<Toggle>().isOn = true;
        ClearInputs(ref procCondAdd, ref procCondRemove, ref procCondInputs);
        ClearInputs(ref endCondAdd, ref endCondRemove, ref endCondInputs);
        frequencyInput.GetComponent<InputField>().text = "0";

        ClearInputs(ref scriptsAdd, ref scriptsRemove, ref scriptsInputs);

        selfDisplaceInput.GetComponent<InputField>().text = "0";
        selfDisplacePerpInput.GetComponent<InputField>().text = "0";
        selfDisplaceParaInput.GetComponent<InputField>().text = "0";
        selfDisplaceAltInput.GetComponent<InputField>().text = "0";

        targetDisplaceInput.GetComponent<InputField>().text = "0";
        targetDisplacePerpInput.GetComponent<InputField>().text = "0";
        targetDisplaceParaInput.GetComponent<InputField>().text = "0";
        targetDisplaceAltInput.GetComponent<InputField>().text = "0";

        ClearInputs(ref followupActionsAdd, ref followupActionsRemove, ref followupActionsInputs);
        ClearInputs(ref targetFollowupActionsAdd, ref targetFollowupActionsRemove, ref targetFollowupActionsInputs);
        ClearInputs(ref targetedFollowupActionsAdd, ref targetedFollowupActionsRemove, ref targetedFollowupActionsInputs);

        ClearInputs(ref conditionsAdd, ref conditionsRemove, ref conditionsInputs);

        ClearInputs(ref cond_scriptsAdd, ref cond_scriptsRemove, ref cond_scriptsInputs);

        cond_selfDisplaceInput.GetComponent<InputField>().text = "0";
        cond_selfDisplacePerpInput.GetComponent<InputField>().text = "0";
        cond_selfDisplaceParaInput.GetComponent<InputField>().text = "0";
        cond_selfDisplaceAltInput.GetComponent<InputField>().text = "0";

        cond_targetDisplaceInput.GetComponent<InputField>().text = "0";
        cond_targetDisplacePerpInput.GetComponent<InputField>().text = "0";
        cond_targetDisplaceParaInput.GetComponent<InputField>().text = "0";
        cond_targetDisplaceAltInput.GetComponent<InputField>().text = "0";

        ClearInputs(ref cond_followupActionsAdd, ref cond_followupActionsRemove, ref cond_followupActionsInputs);
        ClearInputs(ref cond_targetFollowupActionsAdd, ref cond_targetFollowupActionsRemove, ref cond_targetFollowupActionsInputs);
        ClearInputs(ref cond_targetedFollowupActionsAdd, ref cond_targetedFollowupActionsRemove, ref cond_targetedFollowupActionsInputs);
    }

    void Start()
    {
        gameEnv = GameObject.Find("GameLogic").GetComponent<GameEnvScript>().gameEnv; 

        orderedElements = new List<GameObject>() {
            nameText,
            relationalText,
            targetedText,
            stacksText,
            instantText,
            procCondText,
            endCondText,
            frequencyText,
            scriptsText,
            selfDisplaceText,
            selfDisplacePerpText,
            selfDisplaceParaText,
            selfDisplaceAltText,
            targetDisplaceText,
            targetDisplacePerpText,
            targetDisplaceParaText,
            targetDisplaceAltText,
            followupActionsText,
            targetFollowupActionsText,
            targetedFollowupActionsText,
            conditionsText,
            cond_scriptsText,
            cond_selfDisplaceText,
            cond_selfDisplacePerpText,
            cond_selfDisplaceParaText,
            cond_selfDisplaceAltText,
            cond_targetDisplaceText,
            cond_targetDisplacePerpText,
            cond_targetDisplaceParaText,
            cond_targetDisplaceAltText,
            cond_followupActionsText,
            cond_targetFollowupActionsText,
            cond_targetedFollowupActionsText,
            loadEffect_button,
            loadEffect_input,
            editEffect_button,
            clearEffect_button
        };

        procCondAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(procCondText, procCondAdd, procCondRemove, procCondInputs)));
        endCondAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(endCondText, endCondAdd, endCondRemove, endCondInputs)));

        scriptsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(scriptsText, scriptsAdd, scriptsRemove, scriptsInputs)));

        followupActionsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(followupActionsText, followupActionsAdd, followupActionsRemove, followupActionsInputs)));
        targetFollowupActionsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(targetFollowupActionsText, targetFollowupActionsAdd, targetFollowupActionsRemove, targetedFollowupActionsInputs)));
        targetedFollowupActionsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(targetedFollowupActionsText, targetedFollowupActionsAdd, targetedFollowupActionsRemove, targetedFollowupActionsInputs)));

        conditionsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(scriptsText, scriptsAdd, scriptsRemove, scriptsInputs)));

        cond_scriptsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(cond_scriptsText, cond_scriptsAdd, cond_scriptsRemove, cond_scriptsInputs)));

        cond_followupActionsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(cond_followupActionsText, cond_followupActionsAdd, cond_followupActionsRemove, cond_followupActionsInputs)));
        cond_targetFollowupActionsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(cond_targetFollowupActionsText, cond_targetFollowupActionsAdd, cond_targetFollowupActionsRemove, cond_targetedFollowupActionsInputs)));
        cond_targetedFollowupActionsAdd.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(cond_targetedFollowupActionsText, cond_targetedFollowupActionsAdd, cond_targetedFollowupActionsRemove, cond_targetedFollowupActionsInputs)));
    
        procCondRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(procCondInputs, procCondAdd, procCondRemove)));
        endCondRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(endCondInputs, endCondAdd, endCondRemove)));

        scriptsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(scriptsInputs, scriptsAdd, scriptsRemove)));

        followupActionsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(followupActionsInputs, followupActionsAdd, followupActionsRemove)));
        targetFollowupActionsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(targetFollowupActionsInputs, targetFollowupActionsAdd, targetFollowupActionsRemove)));
        targetedFollowupActionsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(targetedFollowupActionsInputs, targetedFollowupActionsAdd, targetedFollowupActionsRemove)));

        conditionsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(conditionsInputs, conditionsAdd, conditionsRemove)));

        cond_scriptsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(cond_scriptsInputs, cond_scriptsAdd, cond_scriptsRemove)));

        cond_followupActionsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(cond_followupActionsInputs, cond_followupActionsAdd, cond_followupActionsRemove)));
        cond_targetFollowupActionsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(cond_targetFollowupActionsInputs, cond_targetFollowupActionsAdd, cond_targetFollowupActionsRemove)));
        cond_targetedFollowupActionsRemove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(cond_targetedFollowupActionsInputs, cond_targetedFollowupActionsAdd, cond_targetedFollowupActionsRemove)));
    }
}
