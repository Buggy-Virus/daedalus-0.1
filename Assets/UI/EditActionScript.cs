using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditActionScript : MonoBehaviour
{
    public float moveDistance = 30;
    public GameEnv gameEnv;

    public GameObject panel;
    public GameObject scriptInputPrefab;
    public GameObject actionInputPrefab;
    public GameObject effectInputPrefab;

    public GameObject name_text;
    public GameObject name_input;
    public GameObject relational_text;
    public GameObject relational_toggle;
    public GameObject targeted_text;
    public GameObject targeted_toggle;
    public GameObject selfTargetable_text;
    public GameObject selfTargetable_toggle;
    public GameObject minRange_text;
    public GameObject minRange_input;
    public GameObject maxRange_text;
    public GameObject maxRange_input;
    public GameObject showConditions_text;
    public GameObject showConditions_add;
    public GameObject showConditions_remove;
    public GameObject availableConditions_text;
    public GameObject availableConditions_add;
    public GameObject availableConditions_remove;
    public GameObject callConditions_text;
    public GameObject callConditions_add;
    public GameObject callConditions_remove;
    public GameObject effects_text;
    public GameObject effects_add;
    public GameObject effects_remove;
    public GameObject aoe_text;
    public GameObject aoe_toggle;
    public GameObject aoeRadius_text;
    public GameObject aoeRadius_input;
    public GameObject aoeRelationalActions_text;
    public GameObject aoeRelationalActions_add;
    public GameObject aoeRelationalActions_remove;
    public GameObject aoeTargetedActions_text;
    public GameObject aoeTargetedActions_add;
    public GameObject aoeTargetedActions_remove;
    public GameObject followupActions_text;
    public GameObject followupActions_add;
    public GameObject followupActions_remove;
    public GameObject targetFollowupActions_text;
    public GameObject targetFollowupActions_add;
    public GameObject targetFollowupActions_remove;
    public GameObject targetedFollowupActions_text;
    public GameObject targetedFollowupActions_add;
    public GameObject targetedFollowupActions_remove;
    public GameObject conditions_text;
    public GameObject conditions_add;
    public GameObject conditions_remove;
    public GameObject conditional_effects_text;
    public GameObject conditional_effects_add;
    public GameObject conditional_effects_remove;
    public GameObject conditional_aoe_text;
    public GameObject conditional_aoe_toggle;
    public GameObject conditional_aoeRadius_text;
    public GameObject conditional_aoeRadius_input;
    public GameObject conditional_aoeRelationalActions_text;
    public GameObject conditional_aoeRelationalActions_add;
    public GameObject conditional_aoeRelationalActions_remove;
    public GameObject conditional_aoeTargetedActions_text;
    public GameObject conditional_aoeTargetedActions_add;
    public GameObject conditional_aoeTargetedActions_remove;
    public GameObject conditional_followupActions_text;
    public GameObject conditional_followupActions_add;
    public GameObject conditional_followupActions_remove;
    public GameObject conditional_targetFollowupActions_text;
    public GameObject conditional_targetFollowupActions_add;
    public GameObject conditional_targetFollowupActions_remove;
    public GameObject conditional_targetedFollowupActions_text;
    public GameObject conditional_targetedFollowupActions_add;
    public GameObject conditional_targetedFollowupActions_remove;
    public GameObject repeat_text;
    public GameObject repeat_toggle;
    public GameObject targetedRepeat_text;
    public GameObject targetedRepeat_toggle;

    public GameObject editAction_button;

    List<GameObject> orderedElements;
    List<GameObject> showConditions_inputs = new List<GameObject>();
    List<GameObject> availableConditions_inputs = new List<GameObject>();
    List<GameObject> callConditions_inputs = new List<GameObject>();
    List<GameObject> effects_inputs = new List<GameObject>();
    List<GameObject> aoeRelationalActions_inputs = new List<GameObject>();
    List<GameObject> aoeTargetedActions_inputs = new List<GameObject>();
    List<GameObject> followupActions_inputs = new List<GameObject>();
    List<GameObject> targetFollowupActions_inputs = new List<GameObject>();
    List<GameObject> targetedFollowupActions_inputs = new List<GameObject>();
    List<GameObject> conditions_inputs = new List<GameObject>();
    List<GameObject> conditional_effects_inputs = new List<GameObject>();
    List<GameObject> conditional_aoeRelationalActions_inputs = new List<GameObject>();
    List<GameObject> conditional_aoeTargetedActions_inputs = new List<GameObject>();
    List<GameObject> conditional_followupActions_inputs = new List<GameObject>();
    List<GameObject> conditional_targetFollowupActions_inputs = new List<GameObject>();
    List<GameObject> conditional_targetedFollowupActions_inputs = new List<GameObject>();

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
            float x_position = name_input.transform.position.x;
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
            float x_position = name_input.transform.position.x;
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
            float x_position = name_input.transform.position.x;
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

    int EditActionHelper(ref string error) {
        Action editAction = new Action();
        editAction.name = name_input.GetComponent<InputField>().text;
        if (editAction.name == "") {
            error += "Action name cannot be blank";
            return 1;
        } 

        editAction.relational = relational_toggle.GetComponent<Toggle>().isOn;
        editAction.targeted = targeted_toggle.GetComponent<Toggle>().isOn;
        editAction.selfTargetable = selfTargetable_toggle.GetComponent<Toggle>().isOn;

        if (!Int32.TryParse(minRange_input.GetComponent<InputField>().text, out editAction.minRange)) {
            error += "Min Range must be an int";
            return 1;
        }
        if (Int32.TryParse(maxRange_input.GetComponent<InputField>().text, out editAction.maxRange)) {
            error += "Min Range must be an int";
            return 1;
        }

        if (showConditions_inputs.Count > 0) {
            editAction.show_conditions = new List<string>();
            foreach (GameObject input in showConditions_inputs) {
                string showConditionInput = input.GetComponent<InputField>().text;
                if (showConditionInput != "") {
                    editAction.show_conditions.Add(showConditionInput);
                }
            }
        }
        
        if (availableConditions_inputs.Count > 0) {
            editAction.available_conditions = new List<string>();
            foreach (GameObject input in availableConditions_inputs) {
                string availableConditionInput = input.GetComponent<InputField>().text;
                if (availableConditionInput != "") {
                    editAction.available_conditions.Add(availableConditionInput);
                }
            }
        }

        if (callConditions_inputs.Count > 0) {
            editAction.call_conditions = new List<string>();
            foreach (GameObject input in callConditions_inputs) {
                string callConditionInput = input.GetComponent<InputField>().text;
                if (callConditionInput != "") {
                    editAction.call_conditions.Add(callConditionInput);
                }
            }
        }

        if (effects_inputs.Count > 0) {
            editAction.effects = new List<Effect>();
            foreach (GameObject input in effects_inputs) {
                string effectInput = input.GetComponent<InputField>().text;
                if (effectInput != "") {
                    if (gameEnv.effectDict.containsKey(effectInput)) {
                        editAction.effects.Add(gameEnv.effectDict[effectInput]);
                    } else {
                        error += "Cannot add effect, no effect of name: " + effectInput;
                        return 1;
                    }
                }
            }
        }

        editAction.aoe = aoe_toggle.GetComponent<Toggle>().isOn;

        if (Int32.TryParse(aoeRadius_input.GetComponent<InputField>().text, out editAction.aoe_radius)) {
            error += "Aoe radius must be an int";
            return 1;
        }

        if (aoeRelationalActions_inputs.Count > 0) {
            editAction.aoe_relational_actions = new List<Action>();
            foreach (GameObject input in aoeRelationalActions_inputs) {
                string aoeRelationalAction = input.GetComponent<InputField>().text;
                if (aoeRelationalAction != "") {
                    if (!gameEnv.actionDict.containsKey(aoeRelationalAction)) {
                        error += "Cannot add aoe relational action, no action of name: " + aoeRelationalAction;
                        return 1;
                    } else if (!gameEnv.actionDict[aoeRelationalAction].relational) {
                        error += "Cannot add aoe relational action, following action is not relatioanl: " + aoeRelationalAction;
                        return 1;
                    } else {
                        editAction.aoe_relational_actions.Add(gameEnv.actionDict[aoeRelationalAction]);
                    }
                }
            }
        }

        if (aoeTargetedActions_inputs.Count > 0) {
            editAction.aoe_targeted_actions = new List<Action>();
            foreach (GameObject input in aoeTargetedActions_inputs) {
                string aoeTargetedAction = input.GetComponent<InputField>().text;
                if (aoeTargetedAction != "") {
                    if (!gameEnv.actionDict.containsKey(aoeTargetedAction)) {
                        error += "Cannot add aoe targeted action, no action of name: " + aoeTargetedAction;
                        return 1;
                    } else if (!gameEnv.actionDict[aoeTargetedAction].relational) {
                        error += "Cannot add aoe targeted action, following action is not targeted: " + aoeTargetedAction;
                        return 1;
                    } else {
                        editAction.aoe_targeted_actions.Add(gameEnv.actionDict[aoeTargetedAction]);
                    }
                }
            }
        }

        if (followupActions_inputs.Count > 0) {
            editAction.followup_actions = new List<Action>();
            foreach (GameObject input in followupActions_inputs) {
                string follwupAction = input.GetComponent<InputField>().text;
                if (follwupAction != "") {
                    if (gameEnv.actionDict.containsKey(follwupAction)) {
                        editAction.followup_actions.Add(gameEnv.actionDict[follwupAction]);
                    } else {
                        error += "Cannot add followup action, no action of name: " + follwupAction;
                        return 1;
                    }
                }
            }
        }

        if (targetFollowupActions_inputs.Count > 0) {
            editAction.target_followup_actions = new List<Action>();
            foreach (GameObject input in targetFollowupActions_inputs) {
                string targetFollowupAction = input.GetComponent<InputField>().text;
                if (targetFollowupAction != "") {
                    if (gameEnv.actionDict.containsKey(targetFollowupAction)) {
                        editAction.target_followup_actions.Add(gameEnv.actionDict[targetFollowupAction]);
                    } else {
                        error += "Cannot add followup action, no action of name: " + targetFollowupAction;
                        return 1;
                    }
                }
            }
        }

        if (targetedFollowupActions_inputs.Count > 0) {
            editAction.targeted_followup_actions = new List<Action>();
            foreach (GameObject input in targetedFollowupActions_inputs) {
                string targetedFollowupAction = input.GetComponent<InputField>().text;
                if (targetedFollowupAction != "") {
                    if (!gameEnv.actionDict.containsKey(targetedFollowupAction)) {
                        error += "Cannot add targeted followup action, no action of name: " + targetedFollowupAction;
                        return 1;
                    } else if (!gameEnv.actionDict[targetedFollowupAction].relational && !gameEnv.actionDict[targetedFollowupAction].targeted) {
                        error += "Cannot add targeted followup action, following action is not targeted or relational: " + targetedFollowupAction;
                        return 1;
                    } else {
                        editAction.targeted_followup_actions.Add(gameEnv.actionDict[targetedFollowupAction]);
                    }
                }
            }
        }

        if (conditions_inputs.Count > 0) {
            editAction.conditions = new List<string>();
            foreach (GameObject input in conditions_inputs) {
                string conditionInput = input.GetComponent<InputField>().text;
                if (conditionInput != "") {
                    editAction.conditions.Add(conditionInput);
                }
            }
        }

        if (conditional_effects_inputs.Count > 0) {
            editAction.conditional_effects = new List<Effect>();
            foreach (GameObject input in conditional_effects_inputs) {
                string effectInput = input.GetComponent<InputField>().text;
                if (effectInput != "") {
                    if (gameEnv.effectDict.containsKey(effectInput)) {
                        editAction.conditional_effects.Add(gameEnv.effectDict[effectInput]);
                    } else {
                        error += "Cannot add conditional effect, no effect of name: " + effectInput;
                        return 1;
                    }
                }
            }
        }

        editAction.conditional_aoe = conditional_aoe_toggle.GetComponent<Toggle>().isOn;

        if (Int32.TryParse(conditional_aoeRadius_input.GetComponent<InputField>().text, out editAction.conditional_aoe_radius)) {
            error += "Aoe conditional radius must be an int";
            return 1;
        }

        if (conditional_aoeRelationalActions_inputs.Count > 0) {
            editAction.conditional_aoe_relational_actions = new List<Action>();
            foreach (GameObject input in conditional_aoeRelationalActions_inputs) {
                string aoeRelationalAction = input.GetComponent<InputField>().text;
                if (aoeRelationalAction != "") {
                    if (!gameEnv.actionDict.containsKey(aoeRelationalAction)) {
                        error += "Cannot add aoe relational action, no action of name: " + aoeRelationalAction;
                        return 1;
                    } else if (!gameEnv.actionDict[aoeRelationalAction].relational) {
                        error += "Cannot add aoe relational action, following action is not relatioanl: " + aoeRelationalAction;
                        return 1;
                    } else {
                        editAction.conditional_aoe_relational_actions.Add(gameEnv.actionDict[aoeRelationalAction]);
                    }
                }
            }
        }

        if (conditional_aoeTargetedActions_inputs.Count > 0) {
            editAction.conditional_aoe_targeted_actions = new List<Action>();
            foreach (GameObject input in conditional_aoeTargetedActions_inputs) {
                string aoeTargetedAction = input.GetComponent<InputField>().text;
                if (aoeTargetedAction != "") {
                    if (!gameEnv.actionDict.containsKey(aoeTargetedAction)) {
                        error += "Cannot add aoe targeted action, no action of name: " + aoeTargetedAction;
                        return 1;
                    } else if (!gameEnv.actionDict[aoeTargetedAction].relational) {
                        error += "Cannot add aoe targeted action, following action is not targeted: " + aoeTargetedAction;
                        return 1;
                    } else {
                        editAction.conditional_aoe_targeted_actions.Add(gameEnv.actionDict[aoeTargetedAction]);
                    }
                }
            }
        }

        if (conditional_followupActions_inputs.Count > 0) {
            editAction.conditional_followup_actions = new List<Action>();
            foreach (GameObject input in conditional_followupActions_inputs) {
                string follwupAction = input.GetComponent<InputField>().text;
                if (follwupAction != "") {
                    if (gameEnv.actionDict.containsKey(follwupAction)) {
                        editAction.conditional_followup_actions.Add(gameEnv.actionDict[follwupAction]);
                    } else {
                        error += "Cannot add followup action, no action of name: " + follwupAction;
                        return 1;
                    }
                }
            }
        }

        if (conditional_targetFollowupActions_inputs.Count > 0) {
            editAction.conditional_target_followup_actions = new List<Action>();
            foreach (GameObject input in conditional_targetFollowupActions_inputs) {
                string targetFollowupAction = input.GetComponent<InputField>().text;
                if (targetFollowupAction != "") {
                    if (gameEnv.actionDict.containsKey(targetFollowupAction)) {
                        editAction.conditional_target_followup_actions.Add(gameEnv.actionDict[targetFollowupAction]);
                    } else {
                        error += "Cannot add followup action, no action of name: " + targetFollowupAction;
                        return 1;
                    }
                }
            }
        }

        if (conditional_targetedFollowupActions_inputs.Count > 0) {
            editAction.conditional_targeted_followup_actions = new List<Action>();
            foreach (GameObject input in conditional_targetedFollowupActions_inputs) {
                string targetedFollowupAction = input.GetComponent<InputField>().text;
                if (targetedFollowupAction != "") {
                    if (!gameEnv.actionDict.containsKey(targetedFollowupAction)) {
                        error += "Cannot add targeted followup action, no action of name: " + targetedFollowupAction;
                        return 1;
                    } else if (!gameEnv.actionDict[targetedFollowupAction].relational && !gameEnv.actionDict[targetedFollowupAction].targeted) {
                        error += "Cannot add targeted followup action, following action is not targeted or relational: " + targetedFollowupAction;
                        return 1;
                    } else {
                        editAction.conditional_targeted_followup_actions.Add(gameEnv.actionDict[targetedFollowupAction]);
                    }
                }
            }
        }

        editAction.repeat = repeat_toggle.GetComponent<Toggle>().isOn;

        editAction.targeted_repeat = targetedRepeat_toggle.GetComponent<Toggle>().isOn;

        gameEnv.actionDict[editAction.name] = editAction;
        return 0;
    }

    public void EditAction() {
        string error = "Edit Action Error:";
        int result = EditActionHelper(ref error);
        if (result == 1) {
            Debug.Log(error);
            gameEnv.console.ConsoleLog(error);
        }
    }

    public void LoadAction(Action load) {
        name_input.GetComponent<InputField>().text = load.name;

        relational_toggle.GetComponent<Toggle>().isOn = load.relational;
        targeted_toggle.GetComponent<Toggle>().isOn = load.targeted;
        selfTargetable_toggle.GetComponent<Toggle>().isOn = load.selfTargetable;

        minRange_input.GetComponent<InputField>().text = load.minRange.ToString();
        maxRange_input.GetComponent<InputField>().text = load.maxRange.ToString();

        if (load.show_conditions != null) {
            foreach(String script in load.show_conditions) {
                AddScriptInput(showConditions_text, showConditions_add, showConditions_remove, showConditions_inputs);
                showConditions_inputs[showConditions_inputs.Count - 1].GetComponent<InputField>().text = script;
            }
        }

        if (load.available_conditions != null) {
            foreach(String script in load.available_conditions) {
                AddScriptInput(availableConditions_text, availableConditions_add, availableConditions_remove, availableConditions_inputs);
                availableConditions_inputs[availableConditions_inputs.Count - 1].GetComponent<InputField>().text = script;
            }
        }

        if (load.call_conditions != null) {
            foreach(String script in load.call_conditions) {
                AddScriptInput(callConditions_text, callConditions_add, callConditions_remove, callConditions_inputs);
                callConditions_inputs[callConditions_inputs.Count - 1].GetComponent<InputField>().text = script;
            }
        }

        if (load.effects != null) {
            foreach(Effect effect in load.effects) {
                AddEffectInput(effects_text, effects_add, effects_remove, effects_inputs);
                effects_inputs[effects_inputs.Count - 1].GetComponent<InputField>().text = effect.name;
            }
        }

        aoe_toggle.GetComponent<Toggle>().isOn = load.aoe;

        aoeRadius_input.GetComponent<InputField>().text = load.aoe_radius.ToString();

        if (load.aoe_relational_actions != null) {
            foreach(Action action in load.aoe_relational_actions) {
                AddEffectInput(aoeRelationalActions_text, aoeRelationalActions_add, aoeRelationalActions_remove, aoeRelationalActions_inputs);
                aoeRelationalActions_inputs[aoeRelationalActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.aoe_targeted_actions != null) {
            foreach(Action action in load.aoe_targeted_actions) {
                AddEffectInput(aoeTargetedActions_text, aoeTargetedActions_add, aoeTargetedActions_remove, aoeTargetedActions_inputs);
                aoeTargetedActions_inputs[aoeTargetedActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.followup_actions != null) {
            foreach(Action action in load.followup_actions) {
                AddEffectInput(followupActions_text, followupActions_add, followupActions_remove, followupActions_inputs);
                followupActions_inputs[followupActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.target_followup_actions != null) {
            foreach(Action action in load.target_followup_actions) {
                AddEffectInput(targetFollowupActions_text, targetFollowupActions_add, targetFollowupActions_remove, targetFollowupActions_inputs);
                targetFollowupActions_inputs[targetFollowupActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.targeted_followup_actions != null) {
            foreach(Action action in load.targeted_followup_actions) {
                AddEffectInput(targetedFollowupActions_text, targetedFollowupActions_add, targetedFollowupActions_remove, targetedFollowupActions_inputs);
                targetedFollowupActions_inputs[targetedFollowupActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.conditions != null) {
            foreach(String script in load.conditions) {
                AddScriptInput(conditions_text, conditions_add, conditions_remove, conditions_inputs);
                conditions_inputs[conditions_inputs.Count - 1].GetComponent<InputField>().text = script;
            }
        }

        if (load.conditional_effects != null) {
            foreach(Effect effect in load.conditional_effects) {
                AddEffectInput(conditional_effects_text, conditional_effects_add, conditional_effects_remove, conditional_effects_inputs);
                conditional_effects_inputs[conditional_effects_inputs.Count - 1].GetComponent<InputField>().text = effect.name;
            }
        }

        conditional_aoe_toggle.GetComponent<Toggle>().isOn = load.conditional_aoe;

        conditional_aoeRadius_input.GetComponent<InputField>().text = load.conditional_aoe_radius.ToString();

        if (load.conditional_aoe_relational_actions != null) {
            foreach(Action action in load.conditional_aoe_relational_actions) {
                AddEffectInput(conditional_aoeRelationalActions_text, conditional_aoeRelationalActions_add, conditional_aoeRelationalActions_remove, conditional_aoeRelationalActions_inputs);
                conditional_aoeRelationalActions_inputs[conditional_aoeRelationalActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.conditional_aoe_targeted_actions != null) {
            foreach(Action action in load.conditional_aoe_targeted_actions) {
                AddEffectInput(conditional_aoeTargetedActions_text, conditional_aoeTargetedActions_add, conditional_aoeTargetedActions_remove, conditional_aoeTargetedActions_inputs);
                conditional_aoeTargetedActions_inputs[conditional_aoeTargetedActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.conditional_followup_actions != null) {
            foreach(Action action in load.conditional_followup_actions) {
                AddEffectInput(conditional_followupActions_text, conditional_followupActions_add, conditional_followupActions_remove, conditional_followupActions_inputs);
                conditional_followupActions_inputs[conditional_followupActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.conditional_target_followup_actions != null) {
            foreach(Action action in load.conditional_target_followup_actions) {
                AddEffectInput(conditional_targetFollowupActions_text, conditional_targetFollowupActions_add, conditional_targetFollowupActions_remove, conditional_targetFollowupActions_inputs);
                conditional_targetFollowupActions_inputs[conditional_targetFollowupActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        if (load.conditional_targeted_followup_actions != null) {
            foreach(Action action in load.conditional_targeted_followup_actions) {
                AddEffectInput(conditional_targetedFollowupActions_text, conditional_targetedFollowupActions_add, conditional_targetedFollowupActions_remove, conditional_targetedFollowupActions_inputs);
                conditional_targetedFollowupActions_inputs[conditional_targetedFollowupActions_inputs.Count - 1].GetComponent<InputField>().text = action.name;
            }
        }

        repeat_toggle.GetComponent<Toggle>().isOn = load.repeat;;

        targetedRepeat_toggle.GetComponent<Toggle>().isOn = load.targeted_repeat;
    }

    public void ClearAction() {

    }

    // Start is called before the first frame update
    void Start()
    {
        orderedElements = new List<GameObject>() {
            name_text,
            relational_text,
            targeted_text,
            selfTargetable_text,
            minRange_text,
            maxRange_text,
            showConditions_text,
            availableConditions_text,
            callConditions_text,
            effects_text,
            aoe_text,
            aoeRadius_text,
            aoeRelationalActions_text,
            aoeTargetedActions_text,
            followupActions_text,
            targetFollowupActions_text,
            targetedFollowupActions_text,
            conditions_text,
            conditional_effects_text,
            conditional_aoe_text,
            conditional_aoeRadius_text,
            conditional_aoeRelationalActions_text,
            conditional_aoeTargetedActions_text,
            conditional_followupActions_text,
            conditional_targetFollowupActions_text,
            conditional_targetedFollowupActions_text,
            repeat_text,
            targetedRepeat_text,
            editAction_button
        };

        showConditions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(showConditions_text, showConditions_add, showConditions_remove, showConditions_inputs)));
        availableConditions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(availableConditions_text, availableConditions_add, availableConditions_remove, availableConditions_inputs)));
        callConditions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(callConditions_text, callConditions_add, callConditions_remove, callConditions_inputs)));

        effects_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddEffectInput(effects_text, effects_add, effects_remove, effects_inputs)));

        aoeRelationalActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(aoeRelationalActions_text, aoeRelationalActions_add, aoeRelationalActions_remove, aoeRelationalActions_inputs)));
        aoeTargetedActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(aoeTargetedActions_text, aoeTargetedActions_add, aoeTargetedActions_remove, aoeTargetedActions_inputs)));

        followupActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(followupActions_text, followupActions_add, followupActions_remove, followupActions_inputs)));
        targetFollowupActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(targetFollowupActions_text, targetFollowupActions_add, targetFollowupActions_remove, targetFollowupActions_inputs)));
        targetedFollowupActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(targetedFollowupActions_text, targetedFollowupActions_add, targetedFollowupActions_remove, targetedFollowupActions_inputs)));

        conditions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddScriptInput(conditions_text, conditions_add, conditions_remove, conditions_inputs)));

        conditional_effects_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddEffectInput(conditional_effects_text, conditional_effects_add, conditional_effects_remove, conditional_effects_inputs)));
        
        conditional_aoeRelationalActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(conditional_aoeRelationalActions_text, conditional_aoeRelationalActions_add, conditional_aoeRelationalActions_remove, conditional_aoeRelationalActions_inputs)));
        conditional_aoeTargetedActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(conditional_aoeTargetedActions_text, conditional_aoeTargetedActions_add, conditional_aoeTargetedActions_remove, conditional_aoeTargetedActions_inputs)));

        conditional_followupActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(conditional_followupActions_text, conditional_followupActions_add, conditional_followupActions_remove, conditional_followupActions_inputs)));
        conditional_targetFollowupActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(conditional_targetFollowupActions_text, conditional_targetFollowupActions_add, conditional_targetFollowupActions_remove, conditional_targetFollowupActions_inputs)));
        conditional_targetedFollowupActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(AddActionInput(conditional_targetedFollowupActions_text, conditional_targetedFollowupActions_add, conditional_targetedFollowupActions_remove, conditional_targetedFollowupActions_inputs)));

        showConditions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(showConditions_inputs, showConditions_add, showConditions_remove)));
        availableConditions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(availableConditions_inputs, availableConditions_add, availableConditions_remove)));
        callConditions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(callConditions_inputs, callConditions_add, callConditions_remove)));
        effects_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(effects_inputs, effects_add, effects_remove)));
        aoeRelationalActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(aoeRelationalActions_inputs, aoeRelationalActions_add, aoeRelationalActions_remove)));
        aoeTargetedActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(aoeTargetedActions_inputs, aoeTargetedActions_add, aoeTargetedActions_remove)));
        followupActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(followupActions_inputs, followupActions_add, followupActions_remove)));
        targetFollowupActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(targetFollowupActions_inputs, targetFollowupActions_add, targetFollowupActions_remove)));
        targetedFollowupActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(targetedFollowupActions_inputs, targetedFollowupActions_add, targetedFollowupActions_remove)));
        conditions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(conditions_inputs, conditions_add, conditions_remove)));
        conditional_effects_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(conditional_effects_inputs, conditional_effects_add, conditional_effects_remove)));
        conditional_aoeRelationalActions_add.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(conditional_aoeRelationalActions_inputs, conditional_aoeRelationalActions_add, conditional_aoeRelationalActions_remove)));
        conditional_aoeTargetedActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(conditional_aoeTargetedActions_inputs, conditional_aoeTargetedActions_add, conditional_aoeTargetedActions_remove)));
        conditional_followupActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(conditional_followupActions_inputs, conditional_followupActions_add, conditional_followupActions_remove)));
        conditional_targetFollowupActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(conditional_targetFollowupActions_inputs, conditional_targetFollowupActions_add, conditional_targetFollowupActions_remove)));
        conditional_targetedFollowupActions_remove.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(RemoveInput(conditional_targetedFollowupActions_inputs, conditional_targetedFollowupActions_add, conditional_targetedFollowupActions_remove)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
