using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditActionScript : MonoBehaviour
{
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

    public GameObject addAction_button;

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

    void moveElements(int elementsToMoveIndex, int amountToMove) {
        foreach (GameObject elem in orderedElements.GetRange(elementsToMoveIndex, orderedElements.Count - elementsToMoveIndex)) {
            elem.transform.position += new Vector3(0, amountToMove, 0);
        }
    }

    public void AddShowConditionInput() {
        // panel.GetComponent<RectTransform>().sizeDelta = new Vector2 (0, -30);

        int elementsToMoveIndex = orderedElements.IndexOf(showConditions_text) + 1;
        moveElements(elementsToMoveIndex, -30);

        GameObject input = GameObject.Instantiate(scriptInputPrefab, panel.transform);
        float x_position = name_input.transform.position.x;
        float y_position = showConditions_text.transform.position.y;
        input.transform.position = new Vector3(x_position, y_position, 0);

        orderedElements.Insert(elementsToMoveIndex, input);
        showConditions_inputs.Add(input);
    }

    public void RemoveShowConditionInput() {
        // panel.GetComponent<RectTransform>().sizeDelta = new Vector2 (0, -30);  
        if (showConditions_inputs.Count != 0) {
            GameObject lastInput = showConditions_inputs[showConditions_inputs.Count - 1];
            int elementsToMoveIndex = orderedElements.IndexOf(lastInput);
            orderedElements.Remove(lastInput);
            showConditions_inputs.Remove(lastInput);
            GameObject.Destroy(lastInput);

            moveElements(elementsToMoveIndex, 30);
        }  
    }


    // Start is called before the first frame update
    void Start()
    {
        orderedElements = new List<GameObject>() {
            name_text,
            name_input,
            relational_text,
            relational_toggle,
            targeted_text,
            targeted_toggle,
            selfTargetable_text,
            selfTargetable_toggle,
            minRange_text,
            minRange_input,
            maxRange_text,
            maxRange_input,
            showConditions_text,
            showConditions_add,
            showConditions_remove,
            availableConditions_text,
            availableConditions_add,
            availableConditions_remove,
            callConditions_text,
            callConditions_add,
            callConditions_remove,
            effects_text,
            effects_add,
            effects_remove,
            aoe_text,
            aoe_toggle,
            aoeRadius_text,
            aoeRadius_input,
            aoeRelationalActions_text,
            aoeRelationalActions_add,
            aoeRelationalActions_remove,
            aoeTargetedActions_add,
            aoeTargetedActions_add,
            aoeTargetedActions_remove,
            followupActions_text,
            followupActions_add,
            followupActions_remove,
            targetFollowupActions_text,
            targetFollowupActions_add,
            targetFollowupActions_remove,
            targetedFollowupActions_text,
            targetedFollowupActions_add,
            targetedFollowupActions_remove,
            conditions_text,
            conditions_add,
            conditions_remove,
            conditional_effects_text,
            conditional_effects_add,
            conditional_effects_remove,
            conditional_aoe_text,
            conditional_aoe_toggle,
            conditional_aoeRadius_text,
            conditional_aoeRadius_input,
            conditional_aoeRelationalActions_text,
            conditional_aoeRelationalActions_add,
            conditional_aoeRelationalActions_remove,
            conditional_aoeTargetedActions_add,
            conditional_aoeTargetedActions_add,
            conditional_aoeTargetedActions_remove,
            conditional_followupActions_text,
            conditional_followupActions_add,
            conditional_followupActions_remove,
            conditional_targetFollowupActions_text,
            conditional_targetFollowupActions_add,
            conditional_targetFollowupActions_remove,
            conditional_targetedFollowupActions_text,
            conditional_targetedFollowupActions_add,
            conditional_targetedFollowupActions_remove,
            repeat_text,
            repeat_toggle,
            targetedRepeat_text,
            targetedRepeat_toggle,
            addAction_button
        };
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
